namespace Intent.Modules.UI.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The configure-drift-check skill's guides to the User Interface designer's recipes for Blazor, Angular and React: a
    /// tested recipe file per technology, the decisions that change it, how each code form is modelled, and how to adapt
    /// it. Written beside the skill, in <c>configure-drift-check/references/</c>, and read only when the code uses the
    /// technology. They describe code, not the designer, so they don't change when the designer does. The recipe files are
    /// tested against code in every style a guide covers (the test project's <c>UiGuideTests</c>), so change one only with
    /// its tests. Edit the text with a plain-text editor: Intent's file tools re-indent raw strings.
    /// </summary>
    internal static class DriftRecipeGuides
    {
        public const string Blazor = """"
# Blazor: the UI recipe file

Start `guardrails/ui.drift.json` from this guide when the UI is Blazor (Server, WebAssembly or a Web App), with or without code-behind. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

`.razor` files, pages with an `@page "/route"` directive, and either `@code { }` blocks or a `.razor.cs` code-behind holding `public partial class Name`. Each `.razor` file is one Component in the model, named as the file is (`Checkout.razor` is `Checkout`); a page is one with the `Page` stereotype and a dialog one with `Dialog`. A layout (`@inherits LayoutComponentBase`) is a Layout.

Every recipe finds the component's files with `file:**/{Component}.razor*`: the `.razor` file and its `.razor.cs` (and `.razor.css`) are read as one, so a fact can be stated in either.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Which projects hold the components? | The projects with `.razor` files that the model describes | Set both `own` entries' `files` to those projects' `.razor` files, such as `src/Acme.Web.Client/**/*.razor`. Leave out a host's Identity pages (`Components/Account/**`) unless they are modelled |
| Are two components named alike in different folders? | Two `Index.razor` files, say | Give the finds a folder: `file:**/{Folder}/{Component}.razor*`, and model the folders as the code has them |
| How does the code call the backend? | An injected client (`IOrdersService.CreateOrderAsync(…)`), MediatR or Mediator (`Send(new CreateOrderCommand(…))`), or an `HttpClient` | The first two are read. For an `HttpClient` called with a URL, see What it can't read |
| Which dialog library? | MudBlazor's `IDialogService.ShowAsync<T>` and `<MudDialog>`, Radzen's `DialogService.OpenAsync<T>`, Blazored's `IModalService.Show<T>` | MudBlazor, Radzen and Blazored open dialogs as `Show…<T>` or `Open…<T>`, which `intent.opens` reads. For another library, add its dialog component to `component.kind`'s third case, such as `<RadzenDialog\\b` |
| Is every page secured unless it says otherwise? | `[Authorize]` in `_Imports.razor`, or `AuthorizeRouteView` with a fallback policy | `security`'s `default` becomes `Secured`, and its `unstated` names where |

## The recipe file

```json
{
  "files": ["src/**/*.razor", "src/**/*.razor.cs"],
  "facts": {
    "component.kind": {
      "find": "file:**/{Component}.razor*",
      "read": [
        { "match": "@inherits LayoutComponentBase\\b|:LayoutComponentBase\\b", "value": "Layout" },
        { "match": "@page\"|\\[Route\\(", "value": "Page" },
        { "match": "<MudDialog\\b|\\bI?MudDialogInstance\\b|\\bBlazoredModalInstance\\b", "value": "Dialog" }
      ],
      "default": "Component",
      "unstated": "no `@page`, `LayoutComponentBase` or dialog instance in {Component}'s files"
    },
    "page.route": {
      "find": "file:**/{Component}.razor*",
      "read": [
        { "match": "@page\"/?([^\"]*)\"" },
        { "match": "\\[Route\\(\"/?([^\"]*)\"\\)\\]" },
        { "match": "\\[Route\\((?!\")[^\\]]*\\]", "unreadable": true }
      ],
      "default": "",
      "unstated": "no `@page` directive or `[Route]` attribute in {Component}'s files"
    },
    "security": {
      "find": "file:**/{Component}.razor*",
      "read": [
        { "match": "\\[AllowAnonymous\\b", "value": "None" },
        { "match": "\\[Authorize\\b", "value": "Secured" }
      ],
      "default": "None",
      "unstated": "no `[Authorize]` on {Component}"
    },
    "security.roles": {
      "find": "file:**/{Component}.razor*",
      "read": [{ "match": "\\[Authorize\\([^\\]]*?Roles=\"([^\"]*\\w)" }],
      "replace": [{ "match": "\\s", "with": "" }],
      "default": "",
      "unstated": "no `Roles` in an `[Authorize]` on {Component}"
    },
    "security.policy": {
      "find": "file:**/{Component}.razor*",
      "read": [
        { "match": "\\[Authorize\\(\"([^\"]*\\w)\"" },
        { "match": "\\[Authorize\\([^\\]]*?Policy=\"([^\"]*\\w)" }
      ],
      "replace": [{ "match": "\\s", "with": "" }],
      "default": "",
      "unstated": "no policy in an `[Authorize]` on {Component}"
    },
    "intent.invokes": {
      "find": "file:**/{Component}.razor*",
      "read": [
        { "match": "\\bnew ({Invoked})\\b" },
        { "match": "\\.{Invoked|trimEnd:Command|trimEnd:Query}(?:Async)?\\(", "value": "{Invoked}" }
      ],
      "default": "",
      "unstated": "no `new {Invoked}` and no call of `{Invoked|trimEnd:Command|trimEnd:Query}Async` in {Component}'s files"
    },
    "intent.navigates": {
      "find": "file:**/{Component}.razor*",
      "read": [
        { "match": "(?:\\bNavigateTo\\(\\$?|\\b[Hh]ref=)\"/?{Route}[\"?#]", "value": "{Page}" },
        { "match": "\\bNavigateTo\\(\\$\"/?{RouteStart}/\\{", "value": "{Page}" }
      ],
      "default": "",
      "unstated": "no `NavigateTo` or `href` to `/{Route}` in {Component}'s files"
    },
    "intent.opens": {
      "find": "file:**/{Component}.razor*",
      "read": [{ "match": "\\b(?:Show|Open)(?:Async)?<({Dialog})>" }],
      "default": "",
      "unstated": "no `ShowAsync<{Dialog}>` in {Component}'s files"
    },
    "intent.uses": {
      "find": "file:**/{Component}.razor",
      "read": [{ "match": "<(?:[\\w.]+\\.)?({Child})\\b" }],
      "default": "",
      "unstated": "no `<{Child}>` in {Component}'s markup"
    }
  },
  "own": [
    { "kind": "page", "find": "file with @page\"", "files": ["src/**/*.razor"] },
    { "kind": "component", "find": "file with ^(?!.*(?:@page\"|<Router\\b|<html\\b)).*<\\w", "files": ["src/**/*.razor"] }
  ]
}
```

The recipes read the code as canonical text: whitespace between two symbols is dropped, so `@page "/cart"` reads `@page"/cart"`, and `@attribute [Authorize]` reads `@attribute[Authorize]`.

## How each form is modelled

The recipes read Blazor in the User Interface designer's terms, so the model must use the same terms (the reverse-engineer-ui skill). The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `component.kind` | `@page`: a page. `<MudDialog>` or a cascading dialog instance: a dialog. `@inherits LayoutComponentBase`: a Layout. Any other `.razor` file: a component | Code, or `default` for a plain component |
| `page.route` | `@page "/orders/{Id:int}"`, or `[Route("/orders/{Id:int}")]` in the code-behind: `Page` → `Route`, as written without the leading `/` | Code |
| `security` | `@attribute [Authorize]`, or `[Authorize]` on the code-behind's class: `Secured`. `[AllowAnonymous]`, or nothing: none | Code for a secured page, `default` for the others |
| `security.roles`, `security.policy` | `[Authorize(Roles = "…")]`, `[Authorize("…")]` or `[Authorize(Policy = "…")]`: `Secured` → `Roles` or `Policy` | Code |
| `intent.invokes` | `OrdersService.CreateOrderAsync(new CreateOrderCommand { … })` or `Mediator.Send(new CreateOrderCommand(…))`: a `Call Service Operation Action` to the Command. `CatalogService.ItemByIdAsync(id)`: to that Service's operation | Code |
| `intent.navigates` | `NavigationManager.NavigateTo("orders")`, `NavigateTo($"customer/edit/{id}")`, or an `href` (`<MudButton Href="/cart">`, `<NavLink href="orders">`): a `Navigation` to the page with that route | Code |
| `intent.opens` | `DialogService.ShowAsync<BrandEditDialog>(…)`: a `Show Dialog` to the dialog | Code |
| `intent.uses` | `<ProductImagePrice … />` in the markup: a `Composition` to that component | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| Clients named otherwise than the operation (`OrdersClient.PostAsync(…)` for `CreateOrderCommand`) | A case for this codebase's naming before the default, such as `\\b{Invoked\|trimEnd:Command}Async\\(`, or model the operation the client calls |
| A navigation helper (`Nav.ToOrder(id)`) | A case for the helper, value `{Page}`, or an `unreadable` case for it so the facts are reported as can't be verified |
| Routes from constants (`NavigateTo(Routes.Orders)`, `@attribute [Route(Routes.Orders)]`) | `page.route` already marks a constant `[Route]` as `unreadable`; give `intent.navigates` one too: `\\bNavigateTo\\((?![\"$])`, `"unreadable": true` |
| A component library's own link component (`<RadzenLink Path="cart">`) | Add its attribute to the first `intent.navigates` case: `\\b(?:[Hh]ref\|Path)=` |

## Ownership

The `own` entries report a `.razor` file the model doesn't describe: `page` for one with `@page`, `component` for any other with markup. The router (`<Router>`), the host page (`<html>`) and `_Imports.razor` (no markup) aren't components.

## What it can't read

- A call made with an `HttpClient` and a URL (`Http.PostAsJsonAsync("api/orders", …)`): no operation is named. Give `intent.invokes` an `unreadable` case for those calls and tell the developer, or have the code call a typed client.
- A navigation built at run time (`NavigateTo(url)`, a menu built from data): give it an `unreadable` case, as above.
- A child component rendered dynamically (`<DynamicComponent Type="…">`, `RenderFragment`s built in code): it reads as missing.
- Something the component does that the model doesn't show: no fact states it, so nothing reports it. Model it when you see it.
"""";

        public const string Angular = """"
# Angular: the UI recipe file

Start `guardrails/ui.drift.json` from this guide when the UI is Angular (standalone components or NgModules). The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

`@Component({ selector: 'app-…', templateUrl: '…' })` classes in `*.component.ts` files, each with a `.component.html` template (and a stylesheet), and routes in `app.routes.ts` or a routing module. Each component is one Component in the model, named as its class is without the `Component` suffix (`CustomerListPageComponent` is `CustomerListPage`); its files are that name in kebab case (`customer-list-page.component.ts`). A routed component is a page, a component injecting `MatDialogRef` a dialog, and the shell with `<router-outlet>` a Layout.

Every component recipe finds the component's files with `file:**/{Component|kebab}.component.*`: the `.ts`, `.html` and stylesheet are read as one, so a fact can be stated in the class or the template. Routes and their guards are read from the routes file.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Where are the routes? | `app.routes.ts`, an `app-routing.module.ts`, or feature routing files | Change the routes file in `component.kind`'s `inherit` and in `page.route`'s, `security`'s and `security.roles`' `find`. Feature routes with a parent path: see What it can't read |
| Which projects hold the components? | The folders with `*.component.ts` files that the model describes | Set the `own` entry's `files` to them |
| What prefix do selectors use? | `selector: 'app-…'` | Change `<app-` in `intent.uses` to the prefix |
| How are routes guarded? | `canActivate: [authGuard]`, with roles in `data: { roles: […] }` | The recipes read both. Roles given to the guard another way (`canActivate: [roleGuard('Admin')]`): a case for that form |
| Is a component's file named otherwise than its class? | `MainLayout` in `main.component.ts` | The finds' second path covers a layout that drops `Layout`; add a path for any other pattern |

## The recipe file

```json
{
  "files": ["src/app/**/*.ts", "src/app/**/*.html"],
  "facts": {
    "component.kind": {
      "find": ["file:**/{Component|kebab}.component.*", "file:**/{Component|trimEnd:Layout|kebab}.component.*"],
      "read": [
        { "match": "<router-outlet\\b", "value": "Layout" },
        { "match": "\\bMatDialogRef<", "value": "Dialog" },
        { "match": "(?<=\\bcomponent:){Component}Component\\b|(?<=\\.then\\(\\w+=>\\w+\\.){Component}Component\\b", "value": "Page" }
      ],
      "inherit": ["file:**/app.routes.ts"],
      "default": "Component",
      "unstated": "no `<router-outlet>` or `MatDialogRef` in {Component}'s files, and no route for `{Component}Component`"
    },
    "page.route": {
      "find": "file:**/app.routes.ts",
      "read": [
        { "match": "\\bpath:'([^']*)',(?:[^{}]|\\{[^{}]*\\})*?\\bcomponent:{Component}Component\\b" },
        { "match": "\\bcomponent:{Component}Component\\b(?:[^{}]|\\{[^{}]*\\})*?\\bpath:'([^']*)'" }
      ],
      "replace": [{ "match": ":(\\w+)", "with": "{$1}" }],
      "default": "",
      "unstated": "no route for `{Component}Component` in `app.routes.ts`"
    },
    "security": {
      "find": "file:**/app.routes.ts",
      "read": [
        { "match": "\\bcanActivate(?=:(?:[^{}]|\\{[^{}]*\\})*?\\bcomponent:{Component}Component\\b)|(?<=\\bcomponent:{Component}Component\\b(?:[^{}]|\\{[^{}]*\\})*?)\\bcanActivate(?=:)", "value": "Secured" }
      ],
      "default": "None",
      "unstated": "no `canActivate` guard on `{Component}Component`'s route"
    },
    "security.roles": {
      "find": "file:**/app.routes.ts",
      "read": [
        { "match": "(?<=\\bcomponent:{Component}Component\\b(?:[^{}]|\\{[^{}]*\\})*?(?:\\{[^{}]*?)?)\\broles:\\['([^\\]]*\\w)'" }
      ],
      "replace": [{ "match": "'\\s*,\\s*'", "with": "," }],
      "default": "",
      "unstated": "no `data: { roles: […] }` on `{Component}Component`'s route"
    },
    "intent.invokes": {
      "find": ["file:**/{Component|kebab}.component.*", "file:**/{Component|trimEnd:Layout|kebab}.component.*"],
      "read": [{ "match": "\\.{Invoked|trimEnd:Command|trimEnd:Query|camel}\\(", "value": "{Invoked}" }],
      "default": "",
      "unstated": "no call of `{Invoked|trimEnd:Command|trimEnd:Query|camel}(…)` in {Component}'s files"
    },
    "intent.navigates": {
      "find": ["file:**/{Component|kebab}.component.*", "file:**/{Component|trimEnd:Layout|kebab}.component.*"],
      "read": [
        { "match": "\\brouterLink=\"/?{Route}[\"?#]", "value": "{Page}" },
        { "match": "\\bnavigate(?:ByUrl)?\\(\\[?'/?{Route}'\\]?\\)", "value": "{Page}" },
        { "match": "\\bnavigate\\(\\['/?{Route|replace:/:','}'\\]", "value": "{Page}" },
        { "match": "(?:\\bnavigate\\(|\\[routerLink\\]=\")\\['/?{RouteStart|replace:/:','}',", "value": "{Page}" },
        { "match": "\\bnavigateByUrl\\(`/?{RouteStart}/\\$\\{", "value": "{Page}" }
      ],
      "default": "",
      "unstated": "no `routerLink` or `navigate` to `/{Route}` in {Component}'s files"
    },
    "intent.opens": {
      "find": ["file:**/{Component|kebab}.component.*", "file:**/{Component|trimEnd:Layout|kebab}.component.*"],
      "read": [{ "match": "\\.open\\(({Dialog})Component\\b" }],
      "default": "",
      "unstated": "no `dialog.open({Dialog}Component, …)` in {Component}'s files"
    },
    "intent.uses": {
      "find": ["file:**/{Component|kebab}.component.*", "file:**/{Component|trimEnd:Layout|kebab}.component.*"],
      "read": [{ "match": "<app-{Child|kebab}\\b", "value": "{Child}" }],
      "default": "",
      "unstated": "no `<app-{Child|kebab}>` in {Component}'s template"
    }
  },
  "own": [
    { "kind": "component", "find": "file with @Component\\(\\{.*?\\}\\)export class (\\w+)", "files": ["src/app/**/*.component.ts"], "name": "$1" }
  ]
}
```

The recipes read the code as canonical text: whitespace between two symbols is dropped, so `{ path: 'customer/add', component: CustomerAddPageComponent }` reads `{path:'customer/add',component:CustomerAddPageComponent}`, and `navigate(['/customer', 'add'])` reads `navigate(['/customer','add'])`. The `replace:/:','` filter writes a route as the array of segments `navigate` takes.

## How each form is modelled

The recipes read Angular in the User Interface designer's terms, so the model must use the same terms (the reverse-engineer-ui skill). The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `component.kind` | A component in the routes: a page. One injecting `MatDialogRef`: a dialog. One with `<router-outlet>`: a Layout. Any other: a component | Code, or `default` for a plain component |
| `page.route` | `{ path: 'customer/edit/:customerId', component: … }`: `Page` → `Route`, with each `:name` parameter as `{name}`: `customer/edit/{customerId}` | Code |
| `security` | `canActivate: [authGuard]` on the route: `Secured`. No guard: none | Code for a guarded page, `default` for the others |
| `security.roles` | `data: { roles: ['Admin', 'Sales'] }` on the route: `Secured` → `Roles` | Code |
| `intent.invokes` | `this.customersService.createCustomer(…)` on the generated or hand-written client: a `Call Service Operation Action` to `CreateCustomerCommand`, or to the Service operation the method is named after | Code |
| `intent.navigates` | `routerLink="/customer-list"`, `router.navigate(['/customer', 'edit', id])` or `navigateByUrl('/customer-list')`: a `Navigation` to the page with that route | Code |
| `intent.opens` | `this.dialog.open(BrandEditDialogComponent, …)`: a `Show Dialog` to `BrandEditDialog` | Code |
| `intent.uses` | `<app-customer-summary-card>` in the template: a `Composition` to `CustomerSummaryCard` | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| Lazy routes (`loadComponent: () => import('./orders.component').then(m => m.OrdersComponent)`) | `component.kind` already reads them. Add to `page.route`: `\\bpath:'([^']*)',(?:[^{}]\|\\{[^{}]*\\})*?\\.{Component}Component\\)` |
| Double quotes (`"customer/add"`) | Change `'` to `\"` in the cases that match strings, and in `replace:/:','` |
| Client methods named otherwise (`customers.post(…)`) | A case for this codebase's naming, or model the operation the method calls |
| A routing module (`RouterModule.forRoot([...])`) | The same cases read it: point the routes file at it |

## Ownership

The `own` entry reports a `*.component.ts` class the model doesn't describe, by its class name.

## What it can't read

- Child routes under a parent with a path (`{ path: 'admin', children: [{ path: 'users', … }] }`): `page.route` reads `users`, not `admin/users`. Give it an `unreadable` case for `children:` routes and tell the developer, or model the child's own path as the code states it.
- A call made with `HttpClient` and a URL (`this.http.post('/api/orders', …)`): no operation is named. Give `intent.invokes` an `unreadable` case and tell the developer.
- A navigation built at run time (`navigate([this.returnUrl])`): it reads as missing; give it an `unreadable` case.
- Something the component does that the model doesn't show: no fact states it, so nothing reports it. Model it when you see it.
"""";

        public const string React = """"
# React: the UI recipe file

Start `guardrails/ui.drift.json` from this guide when the UI is React with React Router, in `.tsx` or `.jsx`. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

Function components (`export function CustomerListPage()`, `export const CustomerCard = (…) =>`) in `.tsx` or `.jsx` files named after them, and routes as `<Route path="…" element={<Page />} />` elements or `createBrowserRouter([{ path, element }])` objects. Each component is one Component in the model, named as the function is. A routed component is a page, one that returns a `<Dialog>` or `<Modal>` a dialog, and one rendering `<Outlet />` a Layout.

Every component recipe finds the component's file with `file:**/{Component}.?sx`. Routes and their guards are read from the router file.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Where are the routes? | `src/App.tsx`, `src/router.tsx` or a `routes` file | Change `src/App.?sx` in `component.kind`'s `inherit` and in `page.route`'s, `security`'s and `security.roles`' `find` |
| Which folders hold the components? | `src/pages`, `src/components`, `src/features/*/components` | Set the `own` entry's `files` to them. Leave out hooks, API clients and tests |
| How are routes guarded? | A wrapper element (`<RequireAuth>`, `<ProtectedRoute>`) around the page | Put the codebase's wrapper names in `security`'s and `security.roles`' cases |
| How does the code call the backend? | A client object (`customersApi.getCustomers()`), or RTK Query or React Query hooks (`useGetCustomersQuery()`) | Both are read. Hooks named otherwise: a case for their pattern |
| Is a component in a folder of its own? | `CustomerCard/index.tsx` | Add a find path: `file:**/{Component}/index.?sx` |

## The recipe file

```json
{
  "files": ["src/**/*.tsx", "src/**/*.jsx"],
  "facts": {
    "component.kind": {
      "find": "file:**/{Component}.?sx",
      "read": [
        { "match": "<Outlet\\b", "value": "Layout" },
        { "match": "\\breturn\\(?<(?:Dialog|Modal)\\b", "value": "Dialog" },
        { "match": "\\belement[=:]\\{?(?:[^{}]|\\{[^{}]*\\})*?<{Component}\\b", "value": "Page" }
      ],
      "inherit": ["file:src/App.?sx"],
      "default": "Component",
      "unstated": "no `<Outlet />` and no root `<Dialog>` in {Component}'s file, and no route for `<{Component} />`"
    },
    "page.route": {
      "find": "file:src/App.?sx",
      "read": [
        { "match": "\\bpath[=:][\"']/?([^\"']*)[\"'],?element[=:]\\{?(?:[^{}]|\\{[^{}]*\\})*?<{Component}\\b" }
      ],
      "replace": [{ "match": ":(\\w+)", "with": "{$1}" }],
      "default": "",
      "unstated": "no route for `<{Component} />` in the router"
    },
    "security": {
      "find": "file:src/App.?sx",
      "read": [
        { "match": "(?<=\\belement[=:]\\{?<)(?:RequireAuth|ProtectedRoute|PrivateRoute)\\b(?=(?:[^{}]|\\{[^{}]*\\})*?<{Component}\\b)", "value": "Secured" }
      ],
      "default": "None",
      "unstated": "no `<RequireAuth>` around `<{Component} />` in the router"
    },
    "security.roles": {
      "find": "file:src/App.?sx",
      "read": [{ "match": "\\broles=\\{\\[\"([^\\]]*\\w)\"\\]\\}(?=[^{}]*?<{Component}\\b)" }],
      "replace": [{ "match": "\"\\s*,\\s*\"", "with": "," }],
      "default": "",
      "unstated": "no `roles` on the guard around `<{Component} />`"
    },
    "intent.invokes": {
      "find": "file:**/{Component}.?sx",
      "read": [
        { "match": "\\.{Invoked|trimEnd:Command|trimEnd:Query|camel}\\(", "value": "{Invoked}" },
        { "match": "\\buse{Invoked|trimEnd:Command|trimEnd:Query}(?:Query|Mutation)\\(", "value": "{Invoked}" }
      ],
      "default": "",
      "unstated": "no call of `{Invoked|trimEnd:Command|trimEnd:Query|camel}(…)` and no `use{Invoked|trimEnd:Command|trimEnd:Query}…` hook in {Component}"
    },
    "intent.navigates": {
      "find": "file:**/{Component}.?sx",
      "read": [
        { "match": "(?:\\bnavigate\\(|\\bto=)[\"'`]/?{Route}[\"'`?#]", "value": "{Page}" },
        { "match": "(?:\\bnavigate\\(|\\bto=\\{)`/?{RouteStart}/\\$\\{", "value": "{Page}" }
      ],
      "default": "",
      "unstated": "no `<Link to>` or `navigate` to `/{Route}` in {Component}"
    },
    "intent.opens": {
      "find": "file:**/{Component}.?sx",
      "read": [{ "match": "<({Dialog})\\b" }],
      "default": "",
      "unstated": "no `<{Dialog}>` in {Component}"
    },
    "intent.uses": {
      "find": "file:**/{Component}.?sx",
      "read": [{ "match": "<({Child})\\b" }],
      "default": "",
      "unstated": "no `<{Child}>` in {Component}"
    }
  },
  "own": [
    { "kind": "component", "find": "file with \\bexport (?:default )?(?:function|const) ([A-Z]\\w*)", "files": ["src/pages/**/*.?sx", "src/components/**/*.?sx"], "name": "$1" }
  ]
}
```

The recipes read the code as canonical text: whitespace between two symbols is dropped, so `<Route path="/customers" element={<CustomerListPage />} />` reads `<Route path="/customers"element={<CustomerListPage/>}/>`.

## How each form is modelled

The recipes read React in the User Interface designer's terms, so the model must use the same terms (the reverse-engineer-ui skill). The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `component.kind` | A component a route's `element` renders: a page. One whose root is `<Dialog>` or `<Modal>`: a dialog. One rendering `<Outlet />`: a Layout. Any other: a component | Code, or `default` for a plain component |
| `page.route` | `<Route path="/customers/:customerId/edit" …>`: `Page` → `Route`, with each `:name` parameter as `{name}`: `customers/{customerId}/edit` | Code |
| `security` | `element={<RequireAuth><CustomerEditPage /></RequireAuth>}`: `Secured`. No guard: none | Code for a guarded page, `default` for the others |
| `security.roles` | `<RequireAuth roles={["Admin", "Sales"]}>`: `Secured` → `Roles` | Code |
| `intent.invokes` | `customersApi.createCustomer(…)` or `useCreateCustomerMutation()`: a `Call Service Operation Action` to `CreateCustomerCommand`, or to the Service operation the method is named after | Code |
| `intent.navigates` | `<Link to="/customers/add">`, `navigate("/customers")` or ``navigate(`/customers/${id}/edit`)``: a `Navigation` to the page with that route | Code |
| `intent.opens` | `<DeleteCustomerDialog open={…} />`: a `Show Dialog` to the dialog | Code |
| `intent.uses` | `<CustomerCard customer={c} />`: a `Composition` to that component | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| React Query (`useQuery({ queryFn: () => customersApi.getCustomers() })`) | Nothing: the client call is read |
| Routes as objects in `createBrowserRouter` | Nothing: `path:` and `element:` are read as `path=` and `element=` are |
| A path from a constant (`<Route path={paths.customers} …>`, `navigate(paths.customers)`) | An `unreadable` case: `\\bpath=\\{`, and `\\bnavigate\\((?![\"'`])` in `intent.navigates` |
| Default exports named otherwise than the file (`export default function Page()` in `CustomerListPage.tsx`) | Name the model's component after the file, and keep the find |

## Ownership

The `own` entry reports an exported component, in the folders it lists, that the model doesn't describe, by its name.

## What it can't read

- Nested routes with relative paths under a parent path (`<Route path="admin"><Route path="users" …/></Route>`): `page.route` reads `users`. Give it an `unreadable` case for nested routes and tell the developer, or model the child's own path as the code states it.
- A call made with `fetch` or `axios` and a URL: no operation is named. Give `intent.invokes` an `unreadable` case and tell the developer.
- A dialog opened through a global store or context (`openDialog("delete")`): it reads as missing; give it an `unreadable` case.
- Something the component does that the model doesn't show: no fact states it, so nothing reports it. Model it when you see it.
"""";
    }
}
