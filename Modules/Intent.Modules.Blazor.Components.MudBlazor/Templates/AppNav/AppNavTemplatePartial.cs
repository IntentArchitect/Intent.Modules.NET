using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.AppNav
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AppNavTemplate : CSharpTemplateBase<LayoutModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Components.MudBlazor.AppNav";

        // Populated by NavigationBarComponentBuilder.SetNavItems while the layout builds; the field's
        // assignment callback below reads it when the file renders (which happens after the build pass).
        private string _itemsInitializer = "[]";

        public AppNavTemplate(IOutputTarget outputTarget, LayoutModel model) : base(TemplateId, outputTarget, model)
        {
            // The Items list is populated by NavigationBarComponentBuilder via SetNavItems during build
            // (that builder holds the BindingManager which resolves each nav item's href). This template
            // only provides the file, so the generated nav stays a single shared source for the
            // interactive MainLayout drawer and the static-SSR ManageLayout drawer.
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("MudBlazor")
                .AddClass("AppNav", @class =>
                {
                    @class.WithComments(
                        """
                        /// <summary>
                        /// Single source of the app navigation items, shared by the interactive MainLayout drawer
                        /// and the static-SSR ManageLayout drawer (both render them via the presentational NavLinks
                        /// component). Keeps the nav identical across shells without duplicating the data.
                        /// </summary>
                        """);
                    @class.Static();
                    @class.AddProperty("IReadOnlyList<NavLinks.NavItem>", "Items",
                        p => p.Static().WithoutSetter().WithInitialValue(_itemsInitializer));
                });
        }

        /// <summary>
        /// Sets the collection-expression initializer for the shared <c>Items</c> list. Called by
        /// NavigationBarComponentBuilder while the layout is being built (before this file renders).
        /// </summary>
        public void SetNavItems(string itemsInitializer)
        {
            _itemsInitializer = itemsInitializer;
        }

        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
