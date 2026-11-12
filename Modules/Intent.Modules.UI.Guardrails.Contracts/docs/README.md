# Intent.UI.Guardrails.Contracts

Keeps the User Interface designer's model true to an existing UI, in Blazor, Angular, React or any other technology. The model describes what each part of the UI is and what it does, not how: every page, dialog, component and layout, each page's route and security, and the backend Commands, Queries and Service operations each one calls, the pages it navigates to, the dialogs it opens and the components it renders. Widgets, operations, properties and mappings aren't modelled.

The module writes no UI code. The model stays true through the drift check (from Intent.Common.Drift, which this module installs), which compares it with the code on every Software Factory run and fails the run when they disagree. It also installs the User Interface designer (Intent.Modelers.UI 1.1.6 or later, with Intent.Modelers.UI.Core for `Show Dialog`). Don't install a UI code generator such as Intent.Blazor or Intent.Angular on the same application: it would generate pages over the hand-written ones.

## AI skill: reverse-engineer-ui

The module generates `skills/reverse-engineer-ui/SKILL.md` into the application's agents folder (the `AI.Context.Skills` output). An AI agent working in Intent Architect uses it to:

- read the UI code and report every page, dialog, component and layout, with each one's route, security, backend calls, navigation, dialogs and children;
- write the drift check's recipe file from those reports, starting from the guide for the technology;
- model the UI in the User Interface designer, following the reference for the installed designer version (`skills/reverse-engineer-ui/references/user-interface-designer-1.md` for Intent.Modelers.UI 1.x);
- finish the drift check until a Software Factory run finds no drift.

Run it after the backend is modelled: the Commands, Queries and Service operations the UI calls must be in this application's Services designer (`reverse-engineer-services`, from Intent.Services.Guardrails.Contracts) or in a Services package this application references.

## Drift check

Once `guardrails/ui.drift.json` exists beside the application's `.application.config`, every Software Factory run compares the UI model with the code and adds what it finds to the UI Area of the application's drift report, `.cache/drift/<application>/DriftReport.mdx`, as Intent.Common.Drift describes. The facts it compares:

| Fact | What the model states |
| --- | --- |
| `component.kind` | Each page, dialog, component and layout, and which of the four it is |
| `page.route` | Each page's route, without its leading `/` |
| `security`, `security.roles`, `security.policy` | Whether each page, or any other secured component, requires a signed-in user, and the roles and policies it names |
| `intent.invokes` | Each Command, Query or Service operation a page, dialog, component or layout calls |
| `intent.navigates` | Each page it navigates to |
| `intent.opens` | Each dialog it opens |
| `intent.uses` | Each component it renders |

A call, navigation, dialog or child the model has and the code doesn't is drift; one the code has and the model doesn't is never reported, so model it when you add it. A page, dialog or component the code has and the model doesn't is reported as not modelled. A page modelled before its code exists fails every run until the code exists: the model and the code change together.

The module's ValidateDriftRecipes module task (`Intent.UI.Guardrails.Contracts.ValidateDriftRecipes`) returns what every UI recipe reads from the code, for the `/validate-drift-recipes` skill.

## Technology guides

The module writes three guides beside the `/configure-drift-check` skill, in `skills/configure-drift-check/references/`. Each holds a recipe file tested against code in every style it covers, the decisions that change it, how each code form is modelled, and how to adapt it:

- `blazor.md`: `.razor` components with `@page` routes, `[Authorize]`, injected clients or Mediator calls, `NavigateTo` and `href`, MudBlazor, Radzen or Blazored dialogs, and code-behind read with its markup;
- `angular.md`: `@Component` classes with their templates, routes and guards in `app.routes.ts`, client service calls, `routerLink` and `router.navigate`, Angular Material dialogs and child selectors;
- `react.md`: function components with React Router routes and guard wrappers, client calls and RTK Query hooks, `<Link to>` and `navigate`, dialogs and child components.

For another technology, the configure-drift-check skill starts from the closest guide.

## The designer version

The User Interface designer is being overhauled. What the drift check reads from today's designer, and the skill's reference for modelling in it, are written for Intent.Modelers.UI 1.x. The facts above, their recipe files and the guides describe the UI, not the designer, so an application's recipe file keeps working when the designer changes.
