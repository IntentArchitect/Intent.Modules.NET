---
name: ui-modeling-blazor.instructions
description: General guidance for AI on how to model UIs for Blazor.
contentHash: 278A67314391DB87589CC6417005B96FCA4AC500125291DE3CAADAEC357979B7
---
## General Rules

- UI Pages / Components should be modeled in the UI Designer.
- Any navigation between Pages should be modeled.
- Pages / Components which are designer to talk to services should model those service interactions.
- If Pages / Components talk to Services the Service Package (where the services are modeled) should be added as a reference in the UI Designer (if its not already there).

## Global navigation

A `Navigation` association from `MainLayout` to a page creates a `Navigation Target End` on `MainLayout`, and that is itself the signal to create a menu item for the page — Navigation items are not modeled separately.

State the item's region in the association's own Comment (e.g. "Navigate to the product list page from the sider menu"); there is no stereotype for it. The closed vocabulary is Header, Sider, Footer and Profile. A comment that names no region defaults to Sider. Use Profile for pages belonging in the profile/account dropdown.

Model a `MainLayout` navigation for each **root-level entry page**: one whose route is stable and needs no route parameters, and which is a top-level capability a user would reach from the application shell (typically list, search or dashboard pages).

Do **not** model one for create, edit, details, manage or any page needing a route parameter (e.g. `/products/{id}`), unless the user explicitly confirms it should be reachable from global navigation. If they do, model it the same way and state the region in the Comment; Profile is not special here.

If it is unclear which pages are root-level entry points, or whether a workflow page belongs in global navigation, ask the user which screens to expose and in which region.

**After changing any `MainLayout` navigation**, the app menu must be reconciled. Never run the `blazor-app-menu` skill yourself and never edit the menu files (`MainLayout*.razor` or the Profile menu file) directly: gather the context that skill needs and hand it to a `coding` sub-agent once, at the end of the unit of work, instructing it to call `use_skill` for `blazor-app-menu` first. The project's general Blazor instructions list what that context must contain.
