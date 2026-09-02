# CONTEXT — Intent.Blazor.Components.MudBlazor

The durable _why_ behind this module's design. Read before modifying it.

## Purpose

Renders a Blazor application's modelled UI using the MudBlazor component library, and owns the application's MudBlazor look — theme CSS, layout wiring, and the default Home page. It layers on top of `Intent.Blazor`, which stands down from anything a component library owns.

## Invariant: `DefaultContentOverride` is the ONLY sanctioned way to seed default page content (2026-08-31)

Default page content is a **bootstrap, not managed output**: the module writes an attractive starting page, and from that moment the developer owns the file. The single mechanism for that is `IAuthPageRazorTemplate.DefaultContentOverride` (and the sibling `StyleContentOverride` for `.razor.css`), set in `OnAfterTemplateRegistrations`.

It is once-off **by construction**, not by a guard the caller has to remember: `ComponentRazorTemplateBase.TransformTextCore()` only consults `DefaultContentOverride` when the output file does not exist on disk; when it does exist, the file is read back and only the managed directive block (`@page`, `<PageTitle>`, `@attribute [Authorize]`) is reconciled.

`StyleContentOverride` has no such built-in rule — `RazorComponentStyleTemplate` pins no `OverwriteBehaviour` and returns the override verbatim — so **every caller must guard it with `File.Exists` itself**. `HomePageDefaultHtmlExtension` and `Intent.Blazor`'s `HomePageDefaultContentFactoryExtension` both do; keep it that way.

The seed strings **omit `@page` and `<PageTitle>`**. `TransformTextCore` injects both from the model's `Page` stereotype on every run and strips them from seeded content first, so including them in the seed is dead weight that drifts from the model.

The seed strings **carry their own indentation**. Content seeded through the template path is written verbatim — nothing reformats it — so a flat raw string ships a flat, unreadable page. (Content pushed through `IChanges` used to be reformatted on write, which is why this was not previously visible.)

## Superseded: the Home page was rewritten post-execution via `IChanges` (2026-08-31)

`HomePageDefaultHtmlExtension.OnAfterTemplateExecution` used to intercept the pending change for `Home.razor` and re-seed the default whenever the file on disk matched the template's output:

```csharp
if (onDisk is null || Normalize(onDisk) == Normalize(templateOutput))
{
    change.ChangeContent(homePageContent, homePageContent);   // clobbers whatever was there
}
```

**This was unsound, and the reason is worth remembering: `change.Content` is derived FROM disk.** `TransformTextCore()` reads the existing file back and only re-manages the directive block, so the comparison asked "did directive reconciliation happen to change any whitespace?" — not "did the developer edit this page?". Two consequences:

1. **Edits survived only by accident.** The reconciliation inserts a blank line between `@page` and `<PageTitle>`, so the strings differed and the `else` branch preserved the file. Any file already carrying that spacing — which is now the steady state every generated application converges on — flipped the branch and had its Home page replaced with the module default.
2. **A modelled Home page could never generate.** The extension intercepted the change on every run, so it either seeded the default or echoed disk; markup designed in the User Interface designer for Home was never emitted.

**Never compare against a pending change's content to decide whether a developer has customized a file.** For a `UserControlledWeave` template that content is a function of the file itself. The only sound test is whether the file exists, which is exactly what `DefaultContentOverride` already does.

The hook is not modelled on the `HomePageDefaultHtmlExtension` Factory Extension element (it carries no stereotypes and no child elements), so deleting the method was sufficient — the Software Factory does not regenerate an empty override.

## Module Interactions

- **`Intent.Blazor`** — owns `ComponentRazorTemplateBase`, `HomeTemplateLookup` and `RazorComponentStyleTemplate`, and seeds a plain-HTML Home page of its own. Its `HomePageDefaultContentFactoryExtension` **stands down when a component library is installed** (`TemplateHelper.ComponentLibraryInstalled`), which is what lets this module's seed take over without the two fighting. If this module ever stops seeding Home, that gate is what needs revisiting.
- **`Intent.Blazor.Authentication`** — independent; it seeds its own account pages through the same `DefaultContentOverride` mechanism. Neither module depends on the other.

## Verifying a change to this module

`dotnet build` proves nothing about generated output. Verify against the consumer test applications under `Tests/` — `Blazor.InteractiveServer.AspNetCoreIdentity` is the cheapest MudBlazor app, and `BlazorNoMudBlazor` is the control for "component library installed but Home page hand-written".

The Software Factory can be run headlessly without opening the Tests solution in the IDE:

```sh
intent-cli update-modules <user> <password> Tests/Intent.Modules.NET.Tests.isln \
    --application-id <app-id> --module "Intent.Blazor.Components.MudBlazor@<version>" --only-if-installed
intent-cli apply-pending-changes <user> <password> Tests/Intent.Modules.NET.Tests.isln --application-id <app-id>
intent-cli ensure-no-outstanding-changes <user> <password> Tests/Intent.Modules.NET.Tests.isln --application-id <app-id>
```

`apply-pending-changes` and `ensure-no-outstanding-changes` accept **one** `--application-id`; `update-modules` accepts several. Modules resolve from `Modules/Intent.Modules` (the Tests solution's "Worktree" repository, gitignored); `dotnet build` writes the `.imod` to `<repo-parent>/.intent-modules`, so copy it across. A **downgrade** currently fails in the nightly CLI (`Intent.ModuleTasks.Host` is missing from the tool package), so the previous behaviour cannot be re-demonstrated that way — verify forwards instead.

Because a rebuilt module at an already-installed version is shadowed by the extracted copy in `Tests/.intent/modules`, each iteration needs its own prerelease bump to be picked up.
