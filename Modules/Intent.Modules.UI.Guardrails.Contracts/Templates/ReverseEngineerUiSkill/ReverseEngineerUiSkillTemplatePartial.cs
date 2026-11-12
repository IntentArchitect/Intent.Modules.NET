using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Modules.UI.Guardrails.Contracts.Drift;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.UI.Guardrails.Contracts.Templates.ReverseEngineerUiSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ReverseEngineerUiSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.UI.Guardrails.Contracts.ReverseEngineerUiSkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ReverseEngineerUiSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "reverse-engineer-ui")
                .FromMarkdown("""
                    ---
                    description: Reverse-engineer an existing UI (Blazor, Angular, React or any other technology) into the Intent Architect User Interface designer at the level of what each part is and what it calls - every page, dialog, component and layout, each page's route and security, the backend Commands, Queries and Service operations each one calls, the pages it navigates to, the dialogs it opens and the components it renders - without widgets, operations or mappings, and configure the drift check that compares the model with the UI code on every Software Factory run. Use when the user has installed the UI Guardrails module (or asks to "model / reverse-engineer the UI / the pages") on a brownfield codebase.
                    short-description: Model an existing UI's pages, dialogs, components and what each calls in the User Interface designer, and check it against the code.
                    argument-hint: "[application name]"
                    ---

                    # Reverse-engineer the UI

                    Goal: the User Interface designer describes the UI **as it is**, at the level of what each part is and what it calls, not how. "The Checkout page calls `CreateOrderCommand` and navigates to Orders" is the level; how it lays out its form, which button makes the call and how the call's fields are filled aren't modelled. Nothing writes UI code: the model stays true through the drift check, which compares it with the code on every Software Factory run. The adoption ends with no drift.

                    Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before modelling.

                    ## 0. Before you start

                    1. `Intent.UI.Guardrails.Contracts` must be installed. If it isn't, stop and ask.
                    2. **The backend must be modelled first.** The Commands, Queries and Service operations the UI calls must exist in a Services designer: this application's (run **reverse-engineer-services** first when this application holds the backend too), or another application's whose Services package this one references. If neither exists, ask the user whether to model the backend first or to leave the calls out, in which case nothing checks them.
                    3. **Never install a UI code generator** (Intent.Blazor, Intent.Angular, Intent.React or their component libraries) on this application, even when the designer or a skill suggests one: it would generate pages over the hand-written ones. The UI is modelled, not generated.
                    4. **Which designer you have.** Look up the installed `Intent.Modelers.UI` version (`listInstalledModules` in `run_solution_query`) for §2's table.

                    ## 1. Read the code (read-only, in parallel)

                    Find the technology: `.razor` files (Blazor), `*.component.ts` classes (Angular), `.tsx` or `.jsx` components with React Router (React), or another (Razor Pages, MVC views, Vue, Svelte). Then hand each area (a folder of pages) to a read-only sub-agent, reporting:

                    - **Each page, dialog, component and layout:** its files, its folder, its name as the code gives it (Blazor's file name, Angular's class name without `Component`, React's function name), and what it is: a **page** (routed), a **dialog** (opened as one), a **layout** (hosts the routed pages: `@inherits LayoutComponentBase`, `<router-outlet>`, `<Outlet />`), or a **component**.
                    - **Each page's route,** as the code states it (`@page "/orders/{Id:int}"`, `{ path: 'orders/:id' }`), and **its security:** whether it requires a signed-in user (`[Authorize]`, a route guard, a wrapper component), with the roles and policies it names.
                    - **What each one calls on the backend:** each call through a client, proxy, Mediator or MediatR request, or data hook, as the Command, Query or Service operation it reaches in the Services model (`OrdersService.CreateOrderAsync(new CreateOrderCommand { … })` reaches `CreateOrderCommand`). A call anywhere in the component (its code-behind, an event handler, a lifecycle method) is the component's. For a call that names no operation (`Http.PostAsJsonAsync("api/orders", …)`, `fetch("/api/orders")`), its URL and verb, to match to an endpoint's route.
                    - **The pages it navigates to,** by the target's route (`NavigateTo`, `href`, `routerLink`, `router.navigate`, `<Link to>`, `navigate(…)`); **the dialogs it opens;** and **the components it renders** (`<ProductCard … />`, `<app-product-card>`).
                    - **One verbatim example of each style** the code uses for each of the above, for the recipes.
                    - **The convention of each kind:** what every page, dialog and component shares (a folder, a file pattern, a base class, a decorator), with one example, or that there is none.

                    **Then write the drift check's recipes, before modelling anything,** while the reports are fresh: load **configure-drift-check**, start `guardrails/ui.drift.json` from its guide for the technology (for another technology, from the closest guide, keeping its facts), and adapt it from the reports without re-reading the code: the `find` for a component's files, the read cases for each style the reports show, and an `own` entry for each kind with a convention. The file turns the check on: every run now fails until the model matches the code, as expected during adoption, and its report lists what isn't modelled yet. §3 finishes it.

                    ## 2. Model it

                    The User Interface designer is being overhauled, so how each concept is modelled depends on its version. Load the reference for the installed `Intent.Modelers.UI` and follow it:

                    | Intent.Modelers.UI | Reference |
                    | --- | --- |
                    | 1.x | [references/user-interface-designer-1.md](references/user-interface-designer-1.md) |

                    From §1's reports, model:

                    - every page, dialog, component and layout, one element each, in folders that mirror the code's;
                    - each page's route, and each secured page's or component's security, roles and policies;
                    - each backend call, navigation, dialog opened and component rendered, from the element itself.

                    Model nothing else: no widgets, views, operations, properties, parameters, events, model definitions or mappings. The model says what each part calls, not how.

                    ## 3. Turn the check on

                    1. **Save the designer.** The check reads the saved model.
                    2. **Run the Software Factory.** It writes no UI code. While there is drift it writes the drift report, `<<DriftReport>>` (in the solution's git-ignored `.cache` folder), and fails the run, so nothing is applied by itself: apply the run's other changes (this module's skills, other designers' output) by path, after reviewing them.
                    3. **Finish the drift check (required).** Load **configure-drift-check** and do its steps 4 to 8 for the UI designer: fix the recipes until no fact is reported as can't be verified and validate-drift-recipes shows every recipe reading the code, then resolve the rest with **sync-model**.
                    - A contradiction is a recipe misreading the code, a modelling slip or real drift: check the code at its location first, fix a recipe that misreads it, and otherwise fix the model, never the code. Where the code looks wrong, say so in the final report.
                    - A page or component the report says isn't modelled: model it (§2), unless it is framework plumbing (a router, a host page, a shared imports file), for which add an exception, asking the user about anything that isn't clearly plumbing.
                    4. **Done** when a run finds no drift and no warnings, so the drift report, `<<DriftReport>>`, has no findings in its UI Area (or no report is staged at all), and the user has approved the model changes (configure-drift-check's step 8). Skip that approval when an orchestrating skill's **User decision** says so.

                    ## Gotchas

                    - **Names verbatim.** Model each name exactly as the code spells it. If the designer renames an element to its own convention, rename it back.
                    - **One element per component.** A component rendered in several places is one element, which each page or component that renders it reaches through its own association.
                    - **Shared chrome is modelled once:** a header, footer or menu is a component its layout renders, not a part of every page.
                    - **No behaviour.** The designer's own rules for new UIs ask for operations, views and mapped calls. For an existing UI they aren't modelled: every interaction goes on the page, dialog, component or layout itself.
                    - **Model and code change together.** Nothing generates UI code, so a page modelled before its code exists fails every run until the code is written.
                    - **What is never checked:** a call, navigation, dialog or child the code gains and the model doesn't have. Model it whenever you see it.
                    """.Replace("<<DriftReport>>", DriftReports.RelativePath(outputTarget.ExecutionContext)));
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
