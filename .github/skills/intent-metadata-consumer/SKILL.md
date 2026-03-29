---
name: intent-metadata-consumer
description: "Use when writing code that reads Intent Architect metadata (stereotypes, attributes, model properties) to drive C# code generation. USE FOR: translating stereotype properties into CSharpFile builder calls; authoring or extending typed *StereotypeExtensions.cs wrappers; writing LINQ queries against typed model collections (ClassModel, AttributeModel, DTOModel, etc.); creating missing typed extension scaffolding from DefinitionId GUIDs. DO NOT USE FOR: event dispatching, DI container registration, appsettings events, or factory extension lifecycle — use intent-module-orchestrator for those."
argument-hint: "[model type] [stereotype name] [target builder action]"
---

# Intent Metadata Consumer

## Workflow: Translate Metadata into Code Generation Logic

The goal is not to check stereotype presence (Discovery) but to **consume** the stereotype's properties and translate them deterministically into C# builder actions (Consumption).

```
model.HasXxx()   ──► guard / filter
model.GetXxx()   ──► accessor to typed wrapper
wrapper.Prop()   ──► drive builder call
```

## Musts

1. **Use generated typed extension methods** for every stereotype access. Never address a stereotype by its string name when a generated extension exists. Generated files match the pattern `*StereotypeExtensions.cs` and are produced by `Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions`.
2. **Access properties through the typed wrapper**, never via raw property-name strings. Call `model.GetMyStereotype()?.SomeProperty()` — not `model.GetStereotype("My Stereotype").GetProperty("Some Property")`.
3. **Map every property type to the right builder action** using the table in this file. Strings do not become comments; Bools do not directly become arguments.
4. **Guard optional accessors with null-conditional operators or TryGet pattern** before calling any property method on the wrapper. A stereotype that is not applied returns `null` from `GetXxx()`.
5. **Use `.AsEnum()` for enum-like stereotype fields** to get a compile-time discriminated switch (`EnumOptions -> EnumOptionsEnum`). Use the `.IsX()` boolean helpers when only one branch is needed.
6. **When a typed extension does not exist yet, resolve stereotypes by `DefinitionId` (GUID), not display name.** Promote this to a typed extension in the next module update.

## Must Nots

1. **Never** call `model.GetStereotype("StereotypeName")` when a typed extension method exists.
2. **Never** call `.GetProperty("PropertyName")` with a string literal for properties that are surfaced by generated wrappers.
3. **Never** branch on `.Value` of a stereotype option property using raw string comparison. Use `.AsEnum()` or `.IsX()` helpers instead.
4. **Never** compose multi-stereotype LINQ queries using only string-based `HasStereotype` predicates when typed `IsXxx()` / `HasXxx()` extension methods are available on the model.
5. **Never** skip the null guard on an optional stereotype accessor when the model is not guaranteed to carry the stereotype.
6. **Never** introduce new display-name string lookups as a fallback strategy (`"My Stereotype"`, `"My Property"`) when a `DefinitionId`-based lookup is available.

## Stereotype Property → C# Builder Action Map

| Property Type | Typed Accessor | Builder Action | Example |
|---|---|---|---|
| **Bool** | `model.GetXxx().IsRequired()` | Conditional `AddAttribute` / gate entire block | `if (model.GetScheduling().DisallowConcurrentExecution()) { @class.AddAttribute(UseType("Quartz.DisallowConcurrentExecution")); }` |
| **Bool** | `model.GetXxx().HasIndexed()` | Conditional `AddAttribute` | `if (attr.HasIndexed()) { prop.AddAttribute(UseType("Redis.OM.Modeling.Indexed")); }` |
| **String (literal)** | `model.GetXxx().Name()` | `AddAttribute(name, value)` / `AddArgument($"\"...\"")` / `WithComments(...)` | `prop.AddAttribute($"JsonPropertyName", a => a.AddArgument($"\"{serializedName}\""));` |
| **String (XML doc)** | `model.GetXxx().ExampleValue()` | `WithComments(xmlComments: ...)` | `prop.WithComments($"/// <example>{model.GetOpenApiSettings().ExampleValue()}</example>");` |
| **Int (constraint)** | `model.GetXxx().MaxLength()` | `AddChainStatement($"MaximumLength({value})")` or `AddArgument($"{value}")` | `chain.AddChainStatement($"MaximumLength({attr.GetTextConstraints().MaxLength()})");` |
| **Int? (optional)** | `model.GetXxx().Priority()` | Null-gate then `AddArgument` | `if (msg.GetXxx().Priority() is {} p) { invoc.AddArgument($"priority: {p}"); }` |
| **Enum wrapper** | `model.GetXxx().TemplatingMethod().AsEnum()` | switch-on-enum | `switch (model.GetFileSettings().TemplatingMethod().AsEnum()) { case T4Template: ... }` |
| **Enum boolean** | `model.GetXxx().TemplatingMethod().IsT4Template()` | Single-branch guard | `if (model.GetFileSettings().TemplatingMethod().IsT4Template()) { ... }` |
| **IElement ref** | `model.GetXxx().Provider()` | Type-resolve + conditional code branch | `var providerEl = pkg.GetDocumentDatabase().Provider(); // branch per element SpecializationType` |

## Strongly-Typed Extension Anatomy

Generated extension files expose three tiers of API for each stereotype:

```csharp
// Tier 1 — existence check (use for filtering/guard)
public static bool HasComponentSettings(this ComponentModel model) { ... }

// Tier 2 — typed accessor (returns wrapper or null)
public static ComponentSettings GetComponentSettings(this ComponentModel model)
{
    var stereotype = model.GetStereotype(ComponentSettings.DefinitionId);
    return stereotype != null ? new ComponentSettings(stereotype) : null;
}

// Tier 3 — TryGet pattern (preferred when consuming inside loops)
public static bool TryGetComponentSettings(
    this ComponentModel model, out ComponentSettings stereotype) { ... }
```

The nested wrapper class exposes per-property methods and a `DefinitionId` GUID constant — **use the GUID, never the display name string**.

## Consuming Enum Options

```csharp
// DON'T — raw string comparison
if (model.GetFileSettings().TemplatingMethod().Value == "T4 Template") { }

// DO — enum helper
if (model.GetFileSettings().TemplatingMethod().IsT4Template()) { }

// DO — discriminated switch on full enum
switch (model.GetFileSettings().TemplatingMethod().AsEnum())
{
    case TemplatingMethodOptionsEnum.T4Template:
        // generate T4 registration
        break;
    case TemplatingMethodOptionsEnum.CSharpFileBuilder:
        // generate CSharpFile registration
        break;
}
```

## Filtering Model Collections

### Simple flag (use typed IsXxx where available)
```csharp
// Prefer typed predicate
var aggregates = domain.GetClassModels()
    .Where(x => x.IsAggregateRoot())
    .ToArray();

// Fall back to HasStereotype only when no typed helper exists
var withTable = domain.GetClassModels()
    .Where(x => x.HasStereotype("Table"))
    .ToArray();
```

### Composite condition (mix of typed and stereotype)
```csharp
// Ranked Complexity Example — Entity Repository filter
var repositoryTargets = _metadataManager.Domain(application).GetClassModels()
    .Where(x => (x.IsAggregateRoot() && (!x.IsAbstract || x.HasStereotype("Table")))
                || x.HasRepository())
    .ToArray();
```

### Property-value filter (GetStereotypeProperty only when no typed wrapper)
```csharp
// Only acceptable when generated typed wrapper does NOT exist for this stereotype
var getOperations = serviceModels
    .SelectMany(x => x.Operations
        .Where(q => q.HasStereotype("Http Settings")
            && q.GetStereotypeProperty<string>("Http Settings", "Verb") == "GET"))
    .Select(o => o.InternalElement)
    .ToArray();
```

## Intent Model Wrapper Hierarchy

Every designer element in Intent Architect is surfaced to code generation as a **typed model wrapper** around the raw `IElement` (from `Intent.SoftwareFactory.SDK`). Stereotype extensions are always written against these typed wrappers, not against raw `IElement`.

### SDK Base Interfaces (`Intent.Metadata.Models` namespace)

| Interface | Contract |
|---|---|
| `IMetadataModel` | `string Id { get; }` |
| `IHasStereotypes` | `IEnumerable<IStereotype> Stereotypes { get; }` |
| `IHasName` | `string Name { get; }` |
| `IHasTypeReference` | `ITypeReference TypeReference { get; }` — leaf/typed nodes |
| `IElementWrapper` | `IElement InternalElement { get; }` — from `Intent.Modules.Common` |
| `IHasFolder` | `FolderModel Folder { get; }` — top-level elements |

### Domain Modeler Model Types (`Intent.Modules.Modelers.Domain`)

| Model Class | Implements | Key Typed Children |
|---|---|---|
| `ClassModel` | `IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IElementWrapper` | `Attributes`, `Operations`, `Constructors`, `AssociatedClasses` |
| `AttributeModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `Class` (parent) |
| `OperationModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `Parameters` |
| `AssociationEndModel` | `ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper` | Directly IS a `ITypeReference` |

### Services Modeler Model Types (`Intent.Modules.Modelers.Services`)

| Model Class | Implements | Key Typed Children |
|---|---|---|
| `DTOModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder` | `Fields` |
| `DTOFieldModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `ParentDto` (parent) |
| `ServiceModel` | `IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IElementWrapper, IAllowCommentModel` | `Operations` |
| `OperationModel` | `IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IProcessingHandlerModel, IElementWrapper` | `Parameters` |

> **Note:** Stereotype extension methods take the specific typed model class (e.g., `this ClassModel model`), not raw `IElement`. This is the key distinction from lower-level SDK usage.

```csharp
// Correct — extension on the typed domain model wrapper
public static bool HasMyStereotype(this ClassModel model)
    => model.HasStereotype(MyStereotype.DefinitionId);

// Wrong — extension on raw IElement loses designer-specific context
public static bool HasMyStereotype(this IElement element) { ... }
```

---

## Creating a Missing Typed Extension

When a stereotype exists in the designer but no `*StereotypeExtensions.cs` has been generated yet, follow these three steps:

### Step 1 — Immediate Fallback (GUID-based, bridge only)

Use the DefinitionId GUID directly with the base helpers from `Intent.Modules.Common`:

```csharp
// Declare the GUID as a constant — never inline it as a raw string
private const string MyStereotypeDefinitionId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

// Existence guard
if (!model.HasStereotype(MyStereotypeDefinitionId)) return;

// Read properties from raw IStereotype
var stereotype = model.GetStereotype(MyStereotypeDefinitionId);
bool isEnabled    = stereotype.GetProperty<bool>("Is Enabled");
string connString = stereotype.GetProperty<string>("Connection String");
int maxRetries    = stereotype.GetProperty<int>("Max Retries");
// Enum-option — raw string comparison acceptable ONLY here in the bridge
string method     = stereotype.GetProperty<string>("Templating Method");
bool isCSharp     = method == "C# File Builder";
```

> The GUID takes precedence over display name. `GetStereotype()` accepts either, but use the GUID to survive display-name renames.

### Step 2 — Promote to a Full Typed Extension

Author a `*StereotypeExtensions.cs` file in the module's `Api/Extensions/` folder. The three-tier API must match the generated pattern exactly so it interoperates with future regeneration:

```csharp
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modelers.Domain.Api; // or .Services.Api depending on target model type

public static class ClassModelStereotypeExtensions   // one file per model type
{
    // Tier 1 — guard / filter
    public static bool HasMyStereotype(this ClassModel model)
        => model.HasStereotype(MyStereotype.DefinitionId);

    // Tier 2 — typed accessor (returns null if not applied)
    public static MyStereotype GetMyStereotype(this ClassModel model)
    {
        var stereotype = model.GetStereotype(MyStereotype.DefinitionId);
        return stereotype != null ? new MyStereotype(stereotype) : null;
    }

    // Tier 3 — TryGet (preferred inside loops)
    public static bool TryGetMyStereotype(this ClassModel model, out MyStereotype stereotype)
    {
        if (!HasMyStereotype(model)) { stereotype = null; return false; }
        stereotype = new MyStereotype(model.GetStereotype(MyStereotype.DefinitionId));
        return true;
    }

    public class MyStereotype
    {
        private readonly IStereotype _stereotype;
        public const string DefinitionId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"; // same GUID

        public MyStereotype(IStereotype stereotype) => _stereotype = stereotype;
        public string Name => _stereotype.Name;

        // Scalar properties — use GetProperty<T>("Property Name")
        public bool   IsEnabled()      => _stereotype.GetProperty<bool>("Is Enabled");
        public string ConnectionString() => _stereotype.GetProperty<string>("Connection String");
        public int    MaxRetries()     => _stereotype.GetProperty<int>("Max Retries");
        public int?   Priority()       => _stereotype.GetProperty<int?>("Priority");

        // Element-reference property
        public IElement Provider() => _stereotype.GetProperty<IElement>("Provider");

        // Enum-option property — wrap in Options class + enum
        public TemplatingMethodOptions TemplatingMethod()
            => new TemplatingMethodOptions(_stereotype.GetProperty<string>("Templating Method"));

        public class TemplatingMethodOptions
        {
            public readonly string Value;
            public TemplatingMethodOptions(string value) { Value = value; }

            public TemplatingMethodOptionsEnum AsEnum() => Value switch
            {
                "C# File Builder" => TemplatingMethodOptionsEnum.CSharpFileBuilder,
                "T4 Template"     => TemplatingMethodOptionsEnum.T4Template,
                _                 => throw new ArgumentOutOfRangeException(nameof(Value), Value, null)
            };

            public bool IsCSharpFileBuilder() => Value == "C# File Builder";
            public bool IsT4Template()        => Value == "T4 Template";
        }

        public enum TemplatingMethodOptionsEnum { CSharpFileBuilder, T4Template }
    }
}
```

### Step 3 — Replace Bridge Code with Typed Calls

Once the extension is in place, replace all GUID/raw-string references:

```csharp
// Before (bridge)
var s = model.GetStereotype(MyStereotypeDefinitionId);
bool isEnabled = s.GetProperty<bool>("Is Enabled");

// After (typed)
if (model.TryGetMyStereotype(out var s))
{
    bool isEnabled = s.IsEnabled();
}
```

---

## Escalation to intent-module-orchestrator

Use **intent-module-orchestrator** when you need to:
- Dispatch `ContainerRegistrationRequest` or `AppSettingRegistrationRequest` via `EventDispatcher`.
- Register `OnBuild`/`AfterBuild` callbacks with priority band ordering.
- Find and modify templates that belong to other modules via `FindTemplateInstance(s)`.

## Source of Truth

### SDK Package
- **`Intent.SoftwareFactory.SDK`** (currently `3.13.0`, targets `netstandard2.0`/`netstandard2.1`)
- Namespace: `Intent.Metadata.Models` — hosts `IHasStereotypes`, `IStereotype`, `IStereotypeProperty<T>`, `IElement`, `ITypeReference`, `IIconModel`

### Common Helpers
- Base `HasStereotype` / `GetStereotype` / `GetProperty<T>` helpers: `Modules/Intent.Modules.Common/Metadata/StereotypeExtensions.cs`
- All `*StereotypeExtensions.cs` files add `using Intent.Modules.Common;` to pull these in

### Canonical Generated Extensions

**`Modules/Intent.Modules.ApplicationTemplate.Builder/Api/ComponentModelStereotypeExtensions.cs`**  
Real-world example of a wrapper with diverse property types:
- `IIconModel Icon()` — icon-reference property
- `bool IncludeByDefault()` / `bool IsRequired()` / `bool IsNew()` — bool properties
- `string Description()` / `string DocumentationUrl()` / `string Tags()` — string properties
- `IElement[] Dependencies()` / `IElement[] Incompatibilities()` — element-array properties (null-coalesced to empty array)
- `RequiredLicenseOptions RequiredLicense()` + `RequiredLicenseOptionsEnum` — enum-option with `.AsEnum()` / `.IsX()` helpers

**`Modules/Intent.Modules.ModuleBuilder.CSharp/Api/Extensions/CSharpTemplateModelStereotypeExtensions.cs`**  
Canonical enum-option example: `TemplatingMethod()` → `TemplatingMethodOptions` → `.AsEnum()` returning `TemplatingMethodOptionsEnum` (`T4Template`, `StringInterpolation`, `CSharpFileBuilder`, `Custom`)

Public repository: <https://github.com/IntentArchitect/Intent.Modules>

---

### `Intent.Modules.Common.Types` — Shared Vocabulary Layer

**Package:** `Intent.Modules.Common.Types` v4.1.3 | `net8.0`  
**Role:** Foundational layer between `Intent.SoftwareFactory.SDK` raw interfaces and designer-specific model types. All modelers (`Domain`, `Services`, etc.) depend on it.

#### Shared Model Types (`Intent.Modules.Common.Types.Api`)

| Type | Specialization | Key Members |
|---|---|---|
| `FolderModel` | `"Folder"` | `Folder` (parent), `Folders` (children), `Name`, `Stereotypes` |
| `EnumModel` | `"Enum"` | `Literals` (`IList<EnumLiteralModel>`), `Folder` |
| `EnumLiteralModel` | `"Enum-Literal"` | `Name`, `Value`, `Stereotypes` |
| `TypeDefinitionModel` | `"Type-Definition"` | Generic/custom type wrappers |
| `AttributeModel` | `"Attribute"` | Shared base; re-specialized by Domain/Services |
| `OperationModel` | `"Operation"` | Shared base; re-specialized by Domain/Services |
| `ParameterModel` | `"Parameter"` | `TypeReference`, `Name` |

#### Folder Navigation Extensions (`FolderExtensions`)

```csharp
// Walk folder chain from model up to root — outermost folder first
IList<FolderModel> folders = model.GetParentFolders();

// Same but as names only (empty strings filtered out)
IList<string> names = model.GetParentFolderNames();

// Walk folder chain looking for a stereotype by name
IStereotype s = model.GetStereotypeInFolders("My Stereotype");
```

#### Template Folder Path Helper (`ModelHasFolderTemplateExtensionsV2`)

```csharp
// Resolves namespace/directory path from IHasFolder on the model
string path = this.GetFolderPath(); // 'this' = IIntentTemplate<TModel> where TModel : IHasFolder
```

#### Primitive Type Checks (`TypeCheckExtensions` + `ElementId`)

Use these when a stereotype has an `IElement`/`ITypeReference` property and you need to branch on the referenced primitive type:

```csharp
// On ICanBeReferencedType (e.g. a type reference element)
bool isString  = typeRef.Element.IsStringType();
bool isGuid    = typeRef.Element.IsGuidType();
bool isInt     = typeRef.Element.IsIntType();
bool isDecimal = typeRef.Element.IsDecimalType();
bool isBool    = typeRef.Element.IsBoolType();
bool isDate    = typeRef.Element.IsDateType();
bool isDateTime = typeRef.Element.IsDateTimeType();
bool isEnum    = typeRef.Element.IsEnumModel();   // see pattern below

// On ITypeReference directly
bool hasString = attribute.TypeReference.HasStringType();
```

`ElementId` constants (in `Intent.Modules.Common`) hold the DefinitionId GUIDs for all built-in primitives (`ElementId.String`, `ElementId.Guid`, `ElementId.Int`, etc.) — use these instead of inline GUID literals when you need raw `GetStereotype(id)` lookups by primitive type.

#### `IsXModel` / `AsXModel` Pattern (`*ModelExtensions`)

Every shared model class in this package exposes:

```csharp
// Check if a referenced type is a known model kind
bool isEnum     = element.IsEnumModel();         // SpecializationTypeId match
bool isFolder   = element.IsFolderModel();

// Cast to typed wrapper — returns null if not the right kind
EnumModel enumModel  = element.AsEnumModel();
FolderModel folder   = element.AsFolderModel();
```

These are the canonical way to branch on what kind of element a type reference resolves to (e.g. determining whether an attribute's type is an enum vs a class vs a primitive).

#### Designer Model Packages

| Module | Package | Key Types |
|---|---|---|
| Domain | `Intent.Modelers.Domain.Api` | `ClassModel`, `AttributeModel`, `OperationModel`, `AssociationEndModel` |
| Services | `Intent.Modelers.Services.Api` | `DTOModel`, `DTOFieldModel`, `ServiceModel`, `OperationModel` |
| Common base | `Intent.Modules.Common` | `IElementWrapper`, `IProcessingHandlerModel`, `IAllowCommentModel`, `StereotypeExtensions` |
| Shared vocabulary | `Intent.Modules.Common.Types` | `FolderModel`, `EnumModel`, `IHasFolder`, `TypeCheckExtensions`, `ElementId` |
