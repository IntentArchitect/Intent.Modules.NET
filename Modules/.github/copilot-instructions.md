# Intent.Modules.NET — Copilot Architecture & Naming Directives

> **Scope:** Applies to all C# module code under `Modules/**`. Loaded automatically by VS Code Copilot.
> **Directives:** Enforces structural integrity, type safety, and mandatory build validation. These rules are **non-negotiable**.

---

## 🏷️ Naming Conventions & Standards

### FactoryExtensions & Templates
* **Suffix:** Use `*FactoryExtension` (e.g., `DomainConstraintsFactoryExtension`). One concern per extension; do not merge unrelated cross-cutting concerns.
* **Template Files:**
    * `*TemplatePartial.cs`: Contains constructor, model wiring, and metadata attachment.
    * `*TemplateBase.cs`: Generated; do not hand-edit except for scaffolded `AfterBuild` callbacks.
* **ID Handling:** Prefer using Template Role names (using string constants) over the template's `TemplateId` constant (static `const string`) for lookups. As last resort using hardcoded `TemplateId` strings.

---

## 🏗️ Architectural Rules

### 1 — Engineering Integrity
* **Scan Before You Name:** Search for existing patterns before creating new classes. `grep_search` → `semantic_search` → then decide. Prefer extending abstractions over parallel ones.
* **Access Modifiers:** Define all new types as `internal` by default. Only use `public` if explicitly required for the external API.
* **Shared Projects:** **Do not introduce `.shproj` / `.projitems`** for new components without explicit approval. Prefer a referenced `.csproj` with `PrivateAssets="All"`.

### 2 — Implementation Quality
* **Eliminate Magic Values:** Use `const` or `static readonly` fields. No inline magic numbers or strings.
* **Modern Strings:** Use **verbatim literals** (`@"..."`) for quotes and **raw string literals** (`"""..."""`) for multi-line blocks.
* **Warning:** Never use global singletons for template-family scope (state must be clearable between Software Factory runs).

### 3 — Template Metadata & Priority Bands
* **Protocol:** Owning templates attach managers in constructors. External extensions use `TryGetMetadata`. Owning templates call `manager.ApplyRules()` in `AfterBuild` at priority `0`.
* **Execution Priorities:**
    | Band | Integer | Usage |
    | :--- | :--- | :--- |
    | **Core** | `0` | Owning template builds primary structure |
    | **Enrichment** | `100` | Same-module cross-cutting additions |
    | **Extension** | `500` | Factory extensions from other modules |
    | **Final** | `1000` | `FindMethod`/`FindClass` on fully-built output |

---

## 🚀 Lifecycle & Validation

### Lifecycle Contract
| Phase | Allowed Actions |
| :--- | :--- |
| `OnBeforeTemplateExecution` | Publish events (Registration Requests). **No CSharpFile mutation.** |
| `OnAfterTemplateRegistrations` | Find instances, schedule callbacks, register into managers. **No event publishing.** |
| `OnBuild` / `AfterBuild` | Mutate `CSharpFile`, read metadata, call `ApplyRules`. |

### Build Validation (Mandatory)
After **every** code change, verify the exit code is `0`:
```powershell
dotnet build "path/to/affected.csproj" --no-incremental --verbosity minimal --nologo
```

## 🛠️ Debugging & Troubleshooting

### Runtime Context Acquisition
If architectural or logic paths are unclear and require runtime context:
1. **Instrument the Code:** Add temporary log entries using `Intent.Utils`.
   * Example: `Logging.Log.Debug("Context: " + variable);`
2. **Request Execution:** Ask the user to run the module/Software Factory.
3. **Analyze Output:** Request the specific log output from the user before proceeding with further code changes.
