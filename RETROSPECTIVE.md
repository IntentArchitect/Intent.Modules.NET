# RETROSPECTIVE.md

Append-only log of gaps, workarounds, and lessons encountered during module builds on this branch. Three buckets: Intent gaps (flag for IA team), Process gaps (update relevant skill), PRD/user gaps (strengthen kickoff questions).

---

## Entry 1 — Interoperability entries missing from Application.Wolverine imodspec

**Bucket:** PRD/user gap  
**Discovered:** feature/wolverine session  
**Symptom:** When users installed `Intent.Application.Wolverine` into an application that already had `Intent.AspNetCore.Controllers`, `Intent.Application.FluentValidation`, or `Intent.DomainEvents` installed, the bridging Wolverine dispatch/validation modules were not auto-installed. Users had to install them manually.  
**Root cause:** The `Intent.Application.Wolverine.imodspec` had no `<interoperability>` section. The MediatR module has this for all its bridging modules, but the Wolverine module was not built with the same pattern.  
**Fix applied:** Added `<interoperability>` block to `Intent.Application.Wolverine.imodspec` detecting 6 platform/feature modules and auto-installing the corresponding Wolverine bridging module:

| Detect | Install |
|---|---|
| `Intent.AspNetCore.Controllers` | `Intent.AspNetCore.Controllers.Dispatch.Wolverine` |
| `Intent.AzureFunctions` | `Intent.AzureFunctions.Dispatch.Wolverine` |
| `Intent.FastEndpoints` | `Intent.FastEndpoints.Dispatch.Wolverine` |
| `Intent.Aws.Lambda.Functions` | `Intent.Aws.Lambda.Functions.Dispatch.Wolverine` |
| `Intent.Application.FluentValidation` | `Intent.Application.Wolverine.FluentValidation` |
| `Intent.DomainEvents` | `Intent.Application.Wolverine.DomainEvents` |

**Skill/kickoff update needed:**  
- `module-kickoff`: Add question — "Which platform modules (Controllers, AzureFunctions, FastEndpoints, Lambda) and feature modules (FluentValidation, DomainEvents) should this module auto-install bridging modules for? Check the MediatR equivalent's `<interoperability>` section and mirror the pattern."  
- `module-wrap-up`: Add checklist item — "Does the imodspec have `<interoperability>` entries for all relevant bridging modules? Mirror the MediatR module's pattern."

---

## Entry 2 — Template registration class must be `public`

**Bucket:** Process gap  
**Discovered:** feature/wolverine session (AwsLambdaFunctions test app)  
**Symptom:** SF ran successfully with 0 staged changes and no Lambda function files generated. No error or warning.  
**Root cause:** `CqrsLambdaFunctionClassTemplateRegistration` was declared `internal`. The SF engine uses `Assembly.GetExportedTypes()` — internal classes are silently skipped.  
**Fix applied:** Changed `internal class` → `public class`. Same root cause was hit previously for `ImplicitControllerTemplateRegistration`.  
**Skill update applied:** Added to `module-increment-loop` SKILL.md.  

---

## Entry 3 — install_or_update_modules is the only valid reinstall path

**Bucket:** Process gap  
**Discovered:** feature/wolverine session  
**Symptom:** After rebuilding a module, assistant attempted to find and manually copy the DLL to the IA module cache.  
**Root cause:** Assistant was not aware that `install_or_update_modules` MCP tool handles cache refresh. CLAUDE.md prohibits reading/writing `.intent` folders but the rule wasn't linked clearly to reinstall workflow.  
**Fix applied:** Added to `module-increment-loop` SKILL.md.  
