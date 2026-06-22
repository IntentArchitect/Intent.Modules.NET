using System.Collections.Generic;
using System.Linq;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Migrations
{
    /// <summary>
    /// Removes the legacy seeded "Menu" (a Header <c>Navigation Menu</c> with Profile / My Account /
    /// Logout) from apps that were created before the seed dropped it (commit f1df5cbf83). Since
    /// <c>AppUserMenu</c> is now always injected, that seeded menu renders as a duplicate top-right
    /// burger. A module update never removes elements an app already owns, so this upgrade migration
    /// cleans it up for existing apps. Brand-new apps never get the menu, so there is nothing to do.
    ///
    /// Matching is deliberately CONSERVATIVE: the menu is only removed when it still matches the
    /// generated seed in every detail (parent Header, menu icon, exactly the three seeded items by
    /// name + icon, and each item navigating to the seeded ExamplePage via its "Link To"). Any
    /// customisation breaks the match and the menu is left untouched.
    /// </summary>
    public class Migration_02_00_00_Pre_01 : IModuleMigration
    {
        // User Interface designer (constant across applications).
        private const string UserInterfaceDesignerId = "f492faed-0665-4513-9853-5a230721786f";

        // Element specialization type ids (verbatim from the seeded UI model).
        private const string NavigationMenuTypeId = "d7282bf2-1626-4b8b-9446-1d530527db06";
        private const string MenuItemTypeId = "adbf2fa8-6833-4c24-960a-31d8a41fd1ed";
        private const string LayoutHeaderTypeId = "a6c3a89e-5932-4ab6-a406-75444f05beee";
        private const string LayoutTypeId = "776a9393-6b23-4a8c-8937-fd7e833fa0ef";

        // "Icon" stereotype and its "Icon Value" property.
        private const string IconStereotypeDefinitionId = "8e1b7033-fd27-495a-a2a7-36b5168f04f5";
        private const string IconValuePropertyId = "7bda044e-8be7-4ac4-98cd-41e97132942f";

        // Seeded icon values - the fingerprint identifying the generated menu and its items.
        private const string MenuIconValue = "e973627f-6106-4763-9459-998016c8c2b3";
        private const string ProfileIconValue = "9257f134-f2a4-446e-aa94-a37565b130c6";
        private const string MyAccountIconValue = "4f0b64a4-01a2-4ea8-9ee5-5785449f159d";
        private const string LogoutIconValue = "caf1cca8-fa22-4dbe-97e0-8ed3e6c51c38";

        // The page each item navigates to in the seed (its "Link To"), via the layout View Binding.
        private const string NavigateExpression = "{NavigateToExamplePage}";
        private const string ViewBindingMappingType = "View Binding";
        private const string LinkToSpecialization = "Link To";

        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_02_00_00_Pre_01(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor.Components.MudBlazor";
        [IntentFully]
        public string ModuleVersion => "2.0.0-pre.1";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(UserInterfaceDesignerId);
            if (designer is null)
            {
                return;
            }

            foreach (var package in designer.GetPackages())
            {
                var changed = false;

                // Candidates: a "Navigation Menu" named "Menu" (GetElementsOfType is recursive).
                var candidateMenus = package.GetElementsOfType(NavigationMenuTypeId)
                    .Where(m => m.Name == "Menu")
                    .ToList();

                foreach (var menu in candidateMenus)
                {
                    if (!IsSeededHeaderMenu(package, menu))
                    {
                        continue; // customised / non-matching -> leave untouched
                    }

                    // Remove the menu element from its parent Header.
                    // NOTE: the layout's View Binding still has mapped ends referencing this menu and
                    // its items. The V2 persistence API exposes Mappings/MappedEnds as read-only, so
                    // those orphaned ends are NOT pruned here. Whether they need pruning (vs. being
                    // harmlessly ignored by the layout code generator) is being verified empirically;
                    // if required, add a V1-model pruning pass.
                    var header = package.GetElementById(menu.ParentFolderId);
                    header?.ChildElements.Remove(menu);
                    changed = true;
                }

                if (changed)
                {
                    package.Save();
                }
            }
        }

        public void Down()
        {
        }

        /// <summary>
        /// True only when <paramref name="menu"/> matches the generated seed in every checked detail.
        /// </summary>
        private static bool IsSeededHeaderMenu(IPackageModelPersistable package, IElementPersistable menu)
        {
            // 1. Parent must be a Layout Header, whose own parent is a Layout (the MainLayout).
            var header = package.GetElementById(menu.ParentFolderId);
            if (header is null || header.SpecializationTypeId != LayoutHeaderTypeId)
            {
                return false;
            }

            var layout = package.GetElementById(header.ParentFolderId);
            if (layout is null || layout.SpecializationTypeId != LayoutTypeId)
            {
                return false;
            }

            // 2. The menu carries the seeded icon.
            if (!HasIconValue(menu, MenuIconValue))
            {
                return false;
            }

            // 3. Children are EXACTLY the three seeded Menu Items (name + icon), nothing else.
            var children = menu.ChildElements.ToList();
            var items = children.Where(c => c.SpecializationTypeId == MenuItemTypeId).ToList();
            if (children.Count != 3 || items.Count != 3)
            {
                return false;
            }

            var profile = items.SingleOrDefault(i => i.Name == "Profile");
            var myAccount = items.SingleOrDefault(i => i.Name == "My Account");
            var logout = items.SingleOrDefault(i => i.Name == "Logout");
            if (profile is null || myAccount is null || logout is null)
            {
                return false;
            }

            if (!HasIconValue(profile, ProfileIconValue)
                || !HasIconValue(myAccount, MyAccountIconValue)
                || !HasIconValue(logout, LogoutIconValue))
            {
                return false;
            }

            // 4. Each item navigates to the seeded ExamplePage via its "Link To" (the pages they
            //    point to), per the layout's View Binding.
            var mappedEnds = layout.Mappings?
                .FirstOrDefault(m => m.Type == ViewBindingMappingType)?
                .MappedEnds?
                .ToList() ?? new List<IElementToElementMappedEndPersistable>();

            return NavigatesToExamplePage(mappedEnds, profile.Id)
                && NavigatesToExamplePage(mappedEnds, myAccount.Id)
                && NavigatesToExamplePage(mappedEnds, logout.Id);
        }

        private static bool HasIconValue(IElementPersistable element, string iconValue)
        {
            return element.Stereotypes.TryGet(IconStereotypeDefinitionId, out var icon)
                && icon!.Properties.TryGet(IconValuePropertyId, out var property)
                && property!.Value == iconValue;
        }

        private static bool NavigatesToExamplePage(
            IEnumerable<IElementToElementMappedEndPersistable> mappedEnds,
            string menuItemId)
        {
            return mappedEnds.Any(end =>
                end.MappingExpression == NavigateExpression
                && end.TargetPath != null
                && end.TargetPath.Any(t => t.Id == menuItemId)
                && end.TargetPath.Any(t => t.Specialization == LinkToSpecialization));
        }
    }
}
