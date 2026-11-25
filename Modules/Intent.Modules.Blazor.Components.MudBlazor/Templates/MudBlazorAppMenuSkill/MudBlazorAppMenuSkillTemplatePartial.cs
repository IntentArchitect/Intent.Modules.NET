using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.MudBlazorAppMenuSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MudBlazorAppMenuSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Components.MudBlazor.MudBlazorAppMenuSkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MudBlazorAppMenuSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-app-menu")
                .FromMarkdown("""
                    ---
                    name: blazor-app-menu
                    description: Populates MainLayoutHeader.razor, MainLayoutSider.razor, MainLayoutFooter.razor, and the Profile menu component (e.g. MainLayoutProfile.razor), either from a natural-language description of the app's menu structure or from modeled Navigation Target Ends (placement resolved from each association's own Comment, not a stereotype), gating individual items by authentication/roles/policy only when the project actually has the Authentication module installed. Also keeps MainLayout.razor's Header/Sider/Footer component references in sync whenever a region is added or removed. Use when asked to add, change, or regenerate navigation/menu items, sider links, header links, or the profile/account menu in a Blazor project using MainLayout/MainLayoutHeader/MainLayoutSider/MainLayoutFooter/Profile-Menu, and whenever a Navigation Target End on MainLayout was added, changed, or removed, or a Sider/Header/Footer region itself was added or removed.
                    paths:
                    ---

                    # App menu (MudBlazor)

                    `MainLayout.razor` composes the region components: `<MainLayoutHeader />`, `<MainLayoutSider />`, `@Body`, and `<MainLayoutFooter />` when a Footer exists. The Header composes the Profile menu component. Each of these files is scaffolded once (`OverwriteDisabled`) with placeholder content, and from then on this skill owns their menu markup. Menus are literal markup: never a shared nav-item model (`NavItem`, `AppNav`, an `Items` loop).

                    ## 1. Read first

                    Samples, in this skill's folder. If any can't be read, stop and say which.
                    - `app-main-layout-sample.razor`: MainLayout's region composition
                    - `app-sider-noauth-sample.razor`, `app-sider-auth-sample.razor`: the Sider without and with gating
                    - `app-header-auth-sample.razor`: Header chrome, a gated Header item, the Profile reference
                    - `app-user-menu-auth-sample.razor`: Profile menu structure

                    Target files:
                    - `Components/Layout/MainLayout.razor` (+ `.razor.cs`). Its AI context is the single list of every Navigation Target End on this Layout (route, roles or policies, and each item's verbatim placement comment), not split by region. It also states whether theming is enabled.
                    - `MainLayoutHeader.razor`, `MainLayoutSider.razor`, `MainLayoutFooter.razor` (+ `.razor.cs`), beside MainLayout.
                    - The Profile file, `{Layout name}{Profile Menu name}.razor` (usually `MainLayoutProfile.razor`): in `Components/Layout/` (NoAuth), `Components/Account/Shared/` (Server Auth), or `Layout/` in the `.Client` project (hosted WASM Auth).
                    - `Program.cs`, or `Startup.cs` where the project has one, plus the host project's for hosted WASM.

                    Any region file may be missing; that's normal. A legacy `ManageLayout.razor` nav is not maintained: leave it alone unless asked.

                    ## 2. Detect the project shape

                    From the startup file, never from the project's name. The registrations are in `Program.cs`
                    in most projects and in `Startup.cs` in others, so check both before concluding NoAuth:
                    - **Auth** if it calls `AddAuthorization()` or `AddAuthorizationCore()` and `AddCascadingAuthenticationState()`. Otherwise **NoAuth**, and `<AuthorizeView>` is never used.
                    - **Hosted WASM** if a host project references a `.Client` project and calls `AddInteractiveWebAssemblyComponents`.

                    ## 3. Parse the items

                    Items come only from the request or from MainLayout's Navigation Target End context. The samples show how to render items, not which items exist: never add one by convention, account items included. Context reads like: "a navigation link to the order list page with route '/order-list' which requires roles 'Admin' or 'Write' (placement comment: "side menu")".

                    | Field | Rule |
                    |---|---|
                    | Label | The page name, title-cased ("order list" → "Order List") |
                    | Href | The literal route |
                    | Icon | Only when described, mapped to the closest `Icons.Material.Filled.*`; otherwise omit `Icon` |
                    | Region | From the placement comment, below |
                    | Gate | From the authorization wording, below |

                    **Region** comes from the item's own placement comment, never from where similar items already sit, and items are never mirrored across regions.

                    | Placement comment | Region |
                    |---|---|
                    | Names Header, Sider, Footer or Profile, or a synonym ("side menu" → Sider, "top nav" or "app bar" → Header, "account menu" → Profile) | That region. If its file is missing, create it (section 4) |
                    | Absent, or doesn't mention a region | Profile if the label or route is account-related ("My Profile", "Change Password", "Manage Account"); otherwise Sider |
                    | Names something that isn't one of the four, or is ambiguous | Sider (or the nearest existing menu file). Create no file, and say in your reply which item fell back and why |

                    An item renders only in its resolved region. The same route appears in two regions only when its comment places it in both.

                    **Gate:**

                    | Context says | Markup |
                    |---|---|
                    | requires roles `A` or `B` | `<AuthorizeView Roles="A,B">`: one comma-joined string, never an array |
                    | requires policy `X` | `<AuthorizeView Policy="X">` |
                    | logged-in users only, or requires authentication | bare `<AuthorizeView>` |
                    | nothing | no wrapper |

                    A gated item is `<AuthorizeView ...><Authorized>item</Authorized></AuthorizeView>`, with no `<NotAuthorized>`: it simply disappears.

                    In a NoAuth project, render gated items ungated and say so in your reply. Never add `AddAuthorization*`, `AddCascadingAuthenticationState` or `<AuthorizeView>` to make gating work.

                    ## 4. Edit the region files

                    - **Additive:** keep existing items the context doesn't mention, unless told to replace the menu.
                    - **Idempotent:** before adding an item, look for it in the same file and the same `<Authorized>` / `<NotAuthorized>` block: by href (case-insensitive, ignoring a trailing `/`), else by label (trimmed, case-insensitive). If it's there, keep it and add nothing.
                    - **Missing file** for a resolved region: create the `.razor` and a trivial `.razor.cs`, following the file-name and namespace conventions of MainLayout and any sibling region file, and copy the fixed chrome verbatim from the matching sample.
                    - Leave a region with no resolved items unchanged.

                    ### Sider: `MainLayoutSider.razor`

                    Follow the noauth or auth Sider sample.
                    - Keep the `<nav class="ux-nav-drawer">` wrapper and its single `<MudNavMenu>`. No `<MudDrawer>`, `DrawerOpen` or `DrawerOpenChanged`; those belonged to an older scaffold.
                    - Each item is `<MudNavLink Href="..." [Icon="..."]>Label</MudNavLink>`. The home link (`/`) gets `Match="NavLinkMatch.All"`; other links omit `Match`.

                    ### Header: `MainLayoutHeader.razor`

                    Follow `app-header-auth-sample.razor`.
                    - Fixed chrome, kept exactly: the `<span onclick="navDrawer.toggle()">` around the menu `MudIconButton`, the brand `MudButton`, and `<MudSpacer />`. They take no `OnClick` or `OnToggle` callbacks.
                    - `<ThemeToggle />` goes after the spacer only when theming is enabled. Never add it when theming is disabled, and never remove it when it's enabled. It takes no callbacks.
                      MainLayout's context states whether theming is enabled; when it doesn't, read it off the project: `MudThemeProvider` bound to `@IsDarkMode`, a `ThemeToggle` component in the project and a toggle already in the Header all mean enabled, while a hardcoded `IsDarkMode="true"` with no `ThemeToggle` anywhere means disabled. Say in your reply which you found and from where — never assume the sample's themed default.
                    - When the Profile file exists, reference it (e.g. `<MainLayoutProfile />`) last, unconditionally: never inside `<AuthorizeView>`, and whether or not Profile has items.
                    - Items are `<MudButton Href="..." Color="Color.Inherit">Label</MudButton>`, between the brand button and `<MudSpacer />`, gated as in section 3.

                    ### Footer: `MainLayoutFooter.razor`

                    Only when items resolve to Footer. Otherwise leave the scaffold, whose body may be just `// Content to be generated`; that's expected. Items are `<MudLink Href="...">` or `<a href>`, not `MudNavLink`, gated as in section 3. There is no Footer sample: a new file starts from that empty state.

                    ### Profile: e.g. `MainLayoutProfile.razor`

                    It is scaffolded as a bare `<UserMenu />`. `UserMenu` is a presentational `<details>` dropdown with `Title`, `Trigger` and `ChildContent`, and holds no auth logic.
                    - Follow `app-user-menu-auth-sample.razor`. Every `<UserMenu>` has a `<Trigger>` holding a `MudIcon`, never a `MudIconButton`. Items go in a `<MudStack Spacing="0">` inside `ChildContent`, each as `<MudButton Href="..." Variant="Variant.Text" Color="Color.Inherit" FullWidth="true">Label</MudButton>`.
                    - **Auth:** wrap the menu in `<AuthorizeView><Authorized>`, as the sample does. Gate an individual item further only when its own context says so.
                    - **NoAuth:** no `<AuthorizeView>` anywhere in the file.
                    - **Account actions** (Login, Register, Manage, Logout) only when the context includes them; the sample's structure shows where they go. Signed-out links (Login, Register) go in a separate `<AuthorizeView><NotAuthorized><UserMenu Title="Account">` block, shaped like the sample's commented-out example. Logout is a POST form to `/Account/Logout` with `<AntiforgeryToken />`, never an `OnClick` or a client-side auth call. An account block left by an earlier run stays unless the request says otherwise.
                    - **Hosted WASM Auth:** check whether the Profile file exists in both the host and the `.Client` project. If it does, keep both copies' items in sync.

                    ## 5. Keep MainLayout's composition in sync

                    Follow `app-main-layout-sample.razor`, and change only which region components `MainLayout.razor` references:
                    - `<MainLayoutHeader />` and `<MainLayoutSider />` are always present. Never remove them.
                    - `<MainLayoutFooter />` is added as the last child of the layout wrapper when `MainLayoutFooter.razor` exists, and removed when it doesn't.
                    - Never add `<MainLayoutProfile />` here; it belongs in the Header.
                    - Leave everything else as it is: providers, `<MudLayout>` / `<MudMainContent>`, classes, and where `@Body` sits. `MudThemeProvider` stays whatever the theming setting; don't reconcile it with `<ThemeToggle />`.
                    - If the references already match the files, don't touch the file.

                    ## Definition of Done

                    - [ ] Auth or NoAuth, and the hosting model, read from `Program.cs` or `Startup.cs` before any edit, and your reply says which file you read and what you found
                    - [ ] Each item's region resolved from its own placement comment; unrecognised placements fell back to Sider and were reported, with no file created for them
                    - [ ] A missing Header, Sider, Footer or Profile file was created (with its `.razor.cs`) when an item resolved to it, with chrome copied from the sample
                    - [ ] Gates are `Roles="A,B"`, `Policy="X"` or bare `<AuthorizeView>`, with `<Authorized>` only; in a NoAuth project nothing is gated or registered, and the ungated items were reported
                    - [ ] `<AuthorizeView>` resolves in every project that uses it: `@using Microsoft.AspNetCore.Components.Authorization` is in its `_Imports.razor` or the file. An unresolved tag only warns (RZ10012) and gates nothing
                    - [ ] Sider wrapper unchanged; home link has `Match="NavLinkMatch.All"`
                    - [ ] Header chrome unchanged; `<ThemeToggle />` present only when theming is enabled; Profile referenced unconditionally when the Profile file exists
                    - [ ] Every `<UserMenu>` has a `<Trigger>`; Profile holds only items resolved to it, and account actions only when the context lists them; hosted WASM copies in sync
                    - [ ] MainLayout references Header and Sider, and Footer exactly when its file exists; no Profile reference; nothing else changed
                    - [ ] No duplicates within a file or auth block; existing items kept
                    - [ ] No invented routes, roles, policies or items; no `NavItem` / `AppNav` model; no `MudDrawer`
                    - [ ] Every project you changed builds, and you ran the build to confirm it

                    """);
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
