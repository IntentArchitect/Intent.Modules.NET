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

## Invariant: a change to any skill's text must be re-benchmarked before it ships (2026-11-24)

The agent skills this module emits are benchmarked by the eval suite in `evals/`, which runs each case twice — once against the frozen baseline (`old_skill`, materialised from the git ref pinned in `evals/baseline/manifest.json`) and once against the working tree (`new_skill`) — then compares the two. `evals/README.md` holds the procedure; `evals/results/` holds the committed reports.

**Parity is the gate, not size.** Per case and model, over 3 runs per arm: no assertion may pass in more than one fewer run than baseline, the build must pass at least as often, and the new arm's mean judge score must be at least the baseline's lowest. Shortening a skill is only finished when that passes.

**A release that changes a sample owes its consumers a note.** The 2.1.1 reduction was planned as skill text only, on the reasoning that `SkillSampleOverwritePolicy` ties sample overwrites to the `SKILL.md` hash and so samples needed no thought. The samples then changed anyway (the ~10% sample cut, and the unrendered-`ErrorMessage` fix), which makes a consumer-facing consequence live: the policy keys the protection on the **skill**, never on the sample, so a consumer who edited a sample but not its `SKILL.md` loses that edit silently, while one who edited a `SKILL.md` receives nothing at all for that skill — neither the reduction nor the behaviour fixes — until they delete its `contentHash` line. State both halves in the release notes of any version that touches a sample. Reversing the policy to key on the sample's own hash would be more correct, but it changes a shared mechanism and must not ship in the same version as the content it protects.

Five things this suite taught the hard way, each of which cost a re-run:

- **A case must fail on an untouched fixture.** `menu-rerun-idempotent` originally asserted only _at most once_, _still present_ and _unchanged_ — all true before any agent ran, so it scored full marks on an untouched app and measured nothing. Step 0 of the README is now that gate. An idempotence case needs at least one genuinely new item in its prompt so that doing nothing fails.
- **A case that passes everything in both arms is unproven, not settled.** `general-styling-handoff` scored 7/7 in both arms for days. Its fixture installed only `updating-styling` and not the `*-ux-theme-sync` it hands off to — `prepare` strips every skill and installs only what `run.json` names — so every run dead-ended at the hand-off, and its checks and graders had been written to match that dead end. Once `run.json`'s `skills` installed both, a correct run failed three checks for doing its job. Full marks in both arms is the signal to go and look.
- **A grader written against behaviour you have never observed encodes your guess, not the rule.** The same case failed every run on "the agent chose the palette itself", which is the intake step `updating-styling` prescribes when no design is supplied and there is nobody to ask. The rule forbids translating design intent _into CSS_; writing design intent into `design.md` is the skill working. Calibrate a grader against real runs before trusting its score.
- **Editing a skill invalidates every case that loads it, not just the one that regressed.** Changing three lines of `blazor-app-menu` made the new arm stale for all seven of its cases at once.
- **Repetition in the old text was sometimes load-bearing, and the fix is accuracy, not narration.** The old app-menu skill named only `Program.cs` for the auth registrations but repeated the check in four places, so agents kept searching until they found them in `Startup.cs`. Stated once, agents checked `Program.cs`, found nothing and concluded NoAuth. The repair was to name both files and require the reply to say which was read — not to restore the repetition.

Judge scores are the noisiest layer: re-judging the same runs against slightly reworded expectations moved scores by a full point. Treat a one-point difference as noise and look at the deterministic checks first.

## Invariant: a CRUD skill reads `ux-tokens.css` and nothing else of the design system (2026-11-25)

The entity page and dialog skills' "Read first" list names only `ux-tokens.css`. It deliberately does **not** name `design.md`, `ux-base.css` or `ux-components.css`, and adding any of them back undoes the largest token saving these skills have. This module and `Intent.Blazor` were changed together and must stay in step — the same line, worded identically, sits in all 12 CRUD skill templates across the two.

The reasoning, in the order it has to be checked before anyone changes it:

- **Measured first.** In a generated app the old list cost ~42 KB per CRUD task (`design.md` 17.2, `ux-tokens.css` 8.9, `ux-base.css` 4.4, `ux-components.css` 11.9) against a skill text of 3.8–5.1 KB — about 60% of the whole payload, and far more than the samples. Cutting it takes a page task from ~69 KB to ~27 KB.
- **`design.md` is not a CRUD input.** It is `mudblazor-ux-theme-sync`'s input — that skill applies it — and documents rationale a CRUD task never acts on. What a CRUD task needs is real token _names_, which live in `ux-tokens.css`. Note `design.md` is shipped by `Intent.Blazor`, not by this module, which is also why `evaltool.py tokens --module mud` cannot run.
- **The page and dialog skills differ, and the difference decides the stylesheets.** The page skills carry §3's "grep each utility class name across `wwwroot/**/*.css`", so a grep already reaches `ux-base.css` and `ux-components.css` when a class is actually wanted. The dialog skills carry no grep and name no utility class — their only styling rule is "inline `Style=` uses `var(--token)`" — so for them those two files supplied nothing but token names. That is why one identical replacement was correct for all 12 rather than two variants.
- **`mudblazor-ux-theme-sync` was not touched.** It still reads all five files in full, correctly.

If a styling regression ever points here, the repair is to state the grep more firmly in §3 — not to restore a read.

**The frontmatter `paths:` globs are a separate mechanism and cost nothing.** They still name `**/design.md`, `**/ux-base.css` and `**/ux-components.css`. `paths` (and its aliases `applyTo` / `appliesTo` / `globs`) is an _inclusion gate_ — the skill is offered when a chat attachment matches one of them — not a preload list.

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
