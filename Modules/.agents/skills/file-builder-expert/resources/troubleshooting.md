# File Builder Troubleshooting

Indexed failure modes for CSharpFile-based templates.

---

## 1. `ToString` Before Build Completion

**Symptom:** `Build() needs to be called before ToString()`  
**Cause:** `TransformText()` was called before the builder lifecycle ran. Usually means `ICSharpFileBuilderTemplate` is not implemented, so the framework never triggers `Build()`.  
**Fix:** Implement `ICSharpFileBuilderTemplate` on the template class. Keep `TransformText` as `return CSharpFile.ToString();` only.

---

## 2. Empty Structural Output

**Symptom:** Exception: `No type or top-level statements were specified`  
**Cause:** `CSharpFile` was constructed but no class, interface, record, enum, or top-level statements were added before `Build()` ran.  
**Fix:** Add at least one structural declaration (e.g. `.AddClass(...)`) in the constructor or in an `OnBuild` callback.

---

## 3. Invalid `OnBuild` Timing

**Symptom:** `This file has already been built`  
**Cause:** `OnBuild(...)` was called after the build lifecycle had already completed — for example, inside an `AfterBuild` callback or in a post-construction hook.  
**Fix:** Register all `OnBuild` callbacks during constructor setup. Never queue new `OnBuild` callbacks from within an `AfterBuild` handler.

---

## 4. Invalid `AfterBuild` Timing

**Symptom:** `The AfterBuild step has already been run for this file`  
**Cause:** `AfterBuild(...)` was registered after the lifecycle already completed the `AfterBuild` phase.  
**Fix:** Register all `AfterBuild` callbacks during constructor setup or from within an `OnBuild` callback that runs while the phase is still open.

---

## 5. Pending Configuration Delegates

**Symptom:** `Pending configurations have not been executed`  
**Cause:** Build lifecycle was interrupted, or callbacks were collected into a queue but never flushed. Can happen when the `CSharpFile` constructor lambda throws before completing.  
**Fix:** Ensure constructor lambdas are deterministic and do not throw. Avoid conditional queue mutations inside `AddClass` / `AddMethod` lambdas where an exception would leave the builder in a half-configured state.

---

## 6. Metadata-Resolution Failures

**Symptom:** `KeyNotFoundException`, `InvalidCastException`, or `NullReferenceException` during post-processing  
**Cause:** Calling `GetMetadata<T>(key)` when the key was never set, or casting to the wrong type.  
**Fix:** Always guard with `HasMetadata` or `TryGetMetadata<T>`. Do not assume metadata set by one template is always present when consumed by another (different execution orders, optional features).

```csharp
// Unsafe:
var flag = method.GetMetadata<bool>("my-key");  // throws if key absent

// Safe:
if (method.TryGetMetadata<bool>("my-key", out var flag) && flag)
{
    // use flag
}
```

---

## 7. Mismatched `TemplateId`

**Symptom:** Template not discovered, or registration resolves to wrong implementation  
**Cause:** `TemplateId` constant in the template class and the value passed to the registration class differ (case-sensitive).  
**Fix:** Define `TemplateId` as a `public const string` in the template and reference it by name in the registration:

```csharp
// Template:
public const string TemplateId = "My.Module.MyTemplate";

// Registration:
public class MyTemplateRegistration : SingleFileTemplateRegistration<MyTemplate>
{
    public override string TemplateId => MyTemplate.TemplateId;  // reference the constant
}
```

---

## 8. Wrong Registration Type

**Symptom:** Template runs once but model-specific files are not generated (or vice versa)  
**Cause:** Using `SingleFileTemplateRegistration` when the template should produce one file per model element.  
**Fix:**

| Scenario | Registration base |
|----------|-------------------|
| One output file | `SingleFileTemplateRegistration` |
| One file per model element | `FilePerModelTemplateRegistration<TModel>` — must also override `GetModels` |
| Event/pipeline-driven | `ITemplateRegistration` directly |
