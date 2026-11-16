# New User Interface designer: what the drift engine needs

**For:** the developer redoing the standard User Interface designer **From:** Joel Turner (drift engine) **Date:** 2026-11-16 **Send back:** one file, `ui-designer-drift-info.md`, and a reference application (section 1)

## Why this is needed

The drift engine checks that an application's code still says what its Intent model says. It learns what a designer's model states from a **pack**. The pack's vocabulary turns each element type, association and stereotype property into a claim about the code, such as "this Component is a page served at `/item/{itemId}`". Its conventions say where each element's code lives. Code readers (Blazor today) then check each claim against the code.

The User Interface designer's pack, `intent.ui`, is keyed on today's exact names: element specializations, association types, and stereotype and property names. Renaming any of them silently stops its checks, and anything new is reported as "no pack says what it states in code" until the pack learns it. So I need the new designer's names, what each one means in code, how today's names map to the new ones, and a working application to prove the new pack on.

Names must be exact, character for character, including spaces, casing and brackets. They are lookup keys.

## What to send back

| # | What                                                                                           | Who produces it                    |
| - | ---------------------------------------------------------------------------------------------- | ---------------------------------- |
| 1 | A reference application that uses the new designer fully, with its code generated and building | You                                |
| 2 | The designer inventory: every element type, association type, stereotype and mapping type      | Your agent (prompt in section 7)   |
| 3 | What each of those states in code, per front-end technology                                    | Your agent drafts it, you check it |
| 4 | Today's names mapped to the new ones                                                           | You                                |
| 5 | Answers to the questions in section 6                                                          | You                                |

Sections 1 and 3 matter most. I can read names off a model. I can't read what they are meant to mean.

## 1. Reference application

Build one Intent application for each front-end technology the designer will generate for at release (Blazor at least).

- Use **every** element type, association type, stereotype and mapping type the designer offers, and set every stereotype property at least once to a value other than its default.
- Include the awkward cases: a page with two routes, a route with a parameter and a constraint, components in folders two deep, a layout, a dialog, a component operation that calls a proxy operation, a navigation that passes a route parameter, a secured page with roles, and everything that's new.
- Run the Software Factory and apply all its output. The application must build and run.
- Keep the model and the code in agreement. I plant the drift myself, and a clean starting point is how I tell a pack bug from real drift. The only hand-written code should be what a team writes in protected bodies, such as a proxy call inside an operation body. List those places.
- If it calls a backend, include the backend application (or its Services package), so that proxy operations resolve to real endpoints.
- List any module settings that change what is generated: naming, file layout, render modes.
- Tell me where it is: repository, branch and `.isln` path.

## 2. Designer inventory

Copy every name exactly as the designer has it.

**Designer**

| Designer name | Designer id | Module id and version | Package type(s) | Replaces today's "User Interface" designer? (same id, same name, or a separate designer) |
| ------------- | ----------- | --------------------- | --------------- | ---------------------------------------------------------------------------------------- |

**Element types**

| Specialization | Allowed in (parent types) | Holds (child types) | Type reference: allowed target types, collection, nullable | Generic types | Mappable to | Traits |
| -------------- | ------------------------- | ------------------- | ---------------------------------------------------------- | ------------- | ----------- | ------ |

**Association types**

| Specialization | Source end: element types | Target end: element types | Default multiplicities | Can the source be an operation or an association end? | Mappings on it |
| -------------- | ------------------------- | ------------------------- | ---------------------- | ----------------------------------------------------- | -------------- |

**Stereotypes**, one row per property

| Stereotype | Applies to | Property | Control type: text, bool, select (with options), element lookup (with target type) | Default |
| ---------- | ---------- | -------- | ---------------------------------------------------------------------------------- | ------- |

**Mapping types**

| Mapping | On (element or association type) | Maps from | Maps to | What is mapped (route parameters, operation inputs, ...) |
| ------- | -------------------------------- | --------- | ------- | -------------------------------------------------------- |

## 3. What each feature states in code

For every row in section 2, and every stereotype property, one line per front-end technology:

| Feature | What it states in code | Example from the reference app (file and snippet) | Literal or computed | Written by the Software Factory or by hand | Notes |
| ------- | ---------------------- | ------------------------------------------------- | ------------------- | ------------------------------------------ | ----- |

Write "nothing", with a reason, for anything that only organises the model, is a view of it, or only tells the Software Factory how to write code (a folder's display options, a diagram, a "generate as partial" switch). Those become declared non-claims, listed as unchecked on purpose rather than as gaps.

What each line needs:

- **The code's name compared with the model's.** Is it exactly the element's name, or does the code add a suffix (`Page`, `Component`) or change the casing of the type or file name? Which settings change it?
- **Where it lives.** The file's path pattern, such as `Components/Pages/{Folder}/{Name}.razor`, and whether there is a separate code-behind (`.razor.cs`).
- **The construct.** For example an `@page "/route"` directive, `<PageTitle>`, `[Parameter]`, `EventCallback<T>`, `[Authorize(Roles = "...")]`, `NavigationManager.NavigateTo("/x")` or `<a href>`, `DialogService.ShowAsync<T>()`, `@inject`, or a child tag such as `<ProductCard />`. For associations, also say whether the code is in the markup, the `@code` block, the code-behind, or another class the component calls (a state store or view model).
- **Literal or computed.** The engine can verify only what the code states literally. A title built from data, or a route assembled from variables, can't be checked. Say which features usually end up computed.
- **Software Factory or by hand.** Which code the Software Factory writes and keeps, and which a developer writes in a protected body. Hand-written code is where drift happens, so it is what the readers must read well.
- **What the default means.** When a property is left at its default, does the code state anything? A default that writes nothing is checked only when it's set.

For each association type, also say what code would mean the same thing if nobody had modelled it: any `NavigateTo`, any `ShowAsync<T>`, any call on a proxy. Teams can turn on "every navigation must be modelled", and that rule needs to know what to look for.

## 4. Today's names, mapped to the new ones

These are all the names the drift engine depends on today. Fill in the two right-hand columns.

For Change, use one of: same, renamed, moved (different parent or designer), reshaped (for example, a stereotype that became an element), split, merged, removed.

**Element types**

| Today                                                 | New name | Change | Notes |
| ----------------------------------------------------- | -------- | ------ | ----- |
| Component                                             |          |        |       |
| Layout                                                |          |        |       |
| Layout Header, Layout Footer                          |          |        |       |
| Display Component (in a Layout Header or Footer)      |          |        |       |
| Model Definition                                      |          |        |       |
| Service Proxy                                         |          |        |       |
| Operation (in a Service Proxy)                        |          |        |       |
| Property (in a Component, Layout or Model Definition) |          |        |       |
| Event Emitter                                         |          |        |       |
| Component Operation                                   |          |        |       |
| Folder                                                |          |        |       |
| Diagram, Comment, Type-Definition                     |          |        |       |

**Stereotypes and their properties**

| Today                                  | New name | Change | Notes |
| -------------------------------------- | -------- | ------ | ----- |
| Page: Route, Title                     |          |        |       |
| Dialog                                 |          |        |       |
| Secured: Roles, Policy, Security Roles |          |        |       |
| Unsecured                              |          |        |       |
| Route Parameter                        |          |        |       |
| Query Parameter                        |          |        |       |
| Bindable                               |          |        |       |
| Composable                             |          |        |       |
| Folder Options                         |          |        |       |

**Association types**

| Today                         | What it means today                                           | New name | Change | Notes |
| ----------------------------- | ------------------------------------------------------------- | -------- | ------ | ----- |
| Composition                   | A component, or one of its operations, uses a child component |          |        |       |
| Navigation                    | A component, or one of its operations, navigates to a page    |          |        |       |
| Show Dialog                   | A component, or one of its operations, opens a dialog         |          |        |       |
| Call Service Operation        | A component uses a proxy                                      |          |        |       |
| Call Service Operation Action | A component operation calls a proxy operation                 |          |        |       |

**Upgrade path**

- Does a version migration convert existing models to the new shape? From which module version?
- Can one application hold old-style and new-style models at once, during the upgrade or permanently?
- Which module version first ships the new designer, and roughly when?

## 5. Today's claims, for reference

These are what the pack checks today. Use them to point out new concepts that don't fit any of them.

- **Components:** kind (page, dialog, layout or component), page route, page title, whether a property is a route parameter, a query parameter or bindable.
- **Links:** uses (a child component), navigates (to a page), opens (a dialog), invokes (a proxy operation), and the target of each.
- **Security:** secured or unsecured, roles, policy.
- **Structure:** types, members, member types, operation returns and parameters.
- **Proxies:** the request is sent, verb, route, API version, headers, RPC method, gRPC service.

## 6. Questions

1. **Technologies.** Which front-end technologies is the new model for: Blazor (Server, WebAssembly, Auto), and any of Angular, React, Vue or MAUI? Is it meant to be technology-neutral? The Java eShop's Vue front end has its own Front End designer with its own checks. If the new designer can model it, I'd move it onto the stock pack.
2. **New concepts.** What does the new designer model that today's doesn't (state stores, forms and validation, view models, actions, route guards, localisation, anything else)? For each one, what in the code shows it (section 3)?
3. **Proxies.** Do service proxies stay in the User Interface designer, move to Services, or go away? How does a proxy operation point at the backend endpoint it calls: a mapping, an association or a stereotype? Does the proxy carry its own verb and route, or take them from the backend operation's Http Settings? Today no stock vocabulary says which backend operation a proxy operation calls; each codebase's own pack fills that in. I'd like the new designer to close that gap.
4. **Routes.** The syntax for parameters and constraints, several routes for one page, optional segments, and any route prefix that comes from a folder, area or parent layout rather than from the page itself.
5. **Security.** Is it still Secured and Unsecured, with Roles, Policy and Security Roles? Does it apply to pages only, or also to components, folders and operations, and is it inherited?
6. **Stability.** When will the names stop changing? Consumers pin a pack's version and move it deliberately, so I'd rather build once against settled names than follow each rename.

## 7. Prompt for your agent

Open an Intent Architect agent chat in the solution that holds the reference application, attach this document, and paste the prompt below. It fills section 2 and drafts section 3. You then check section 3 and fill in sections 4 and 6.

```text
Produce ui-designer-drift-info.md in the root of this solution, following sections 2 and 3 of the attached handover document. Use its tables and headings as they are.

1. Load the exploring-the-model and scripting-the-model skills, and follow them.
2. In the reference application, read the new User Interface designer's schema: every element type, association type, stereotype (each property with its control type, options, default and lookup target type) and mapping type. Copy names exactly: don't change spacing, casing or brackets.
3. Fill in section 2's tables from that schema alone.
4. Draft section 3. For each feature, find where the reference application's generated code states it, and give the file path and a short snippet. Where the designer's or the generating modules' templates are in this repository, read them to confirm the naming and path rules and any module setting that changes them. Mark each line "from code" or "from template". Where you can't find a feature in the code, write "not found". Don't guess what a feature means.
5. Don't change the model, run the Software Factory or edit any code.
```
