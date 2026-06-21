# C# File Builder Cheat Sheet & Patterns

## Minimal Template Shape

```csharp
[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class SampleTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "My.Module.SampleTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public SampleTemplate(IOutputTarget outputTarget, object model = null)
        : base(TemplateId, outputTarget, model)
    {
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
            .AddUsing("System")
            .AddClass("Sample", @class =>
            {
                @class.AddConstructor(ctor =>
                    ctor.AddParameter("string", "value", p => p.IntroduceReadonlyField()));
                @class.AddMethod("void", "DoWork", method =>
                    method.AddStatement("// TODO"));
            });
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }
    [IntentManaged(Mode.Fully)] protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();
    [IntentManaged(Mode.Fully)] public override string TransformText() => CSharpFile.ToString();
}
```

## Resolving Other Templates' Namespaces

```csharp
var configTemplate = application.FindTemplateInstance<IClassProvider>(NServiceBusConfigurationTemplate.TemplateId);
if (configTemplate != null)
{
    file.AddUsing(configTemplate.Namespace);
}
```

## Conditional AddUsing Patterns

```csharp
// Branch-based:
if (useTopLevelStatements)
{
    CSharpFile.AddUsing(this.GetNamespace());
}

// Dependency-driven:
foreach (var templateDependency in @event.TemplateDependencies)
{
    var template = GetTemplate<IClassProvider>(templateDependency);
    if (template != null)
    {
        AddUsing(template.Namespace);
    }
}

// Namespace collection:
foreach (var ns in @event.RequiredNamespaces)
{
    AddUsing(ns);
}

// Only introduce the namespace when this exact type is needed:
method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
```

## Registration Quick-Ref

| Template type | Registration base |
|---|---|
| Single output file | `SingleFileTemplateRegistration` |
| One file per model | `FilePerModelTemplateRegistration<TModel>` — override `GetModels` |
| One file for all models | `SingleFileListModelTemplateRegistration<TModel>` — override `GetModels` |
| Event/pipeline driven | `ITemplateRegistration` |
