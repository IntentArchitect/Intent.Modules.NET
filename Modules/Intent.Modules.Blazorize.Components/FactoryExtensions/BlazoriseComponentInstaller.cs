using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates;
using Intent.Modules.Blazor.Components.Core.Templates.RazorComponent;
using Intent.Modules.Blazorize.Components.ComponentRenderer;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazorize.Components.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BlazoriseComponentInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazorize.Components.BlazoriseComponentInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            DefaultRazorComponentBuilderProvider.Register(FormModel.SpecializationTypeId, (provider, componentTemplate) => new FormComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TextInputModel.SpecializationTypeId, (provider, componentTemplate) => new TextInputComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ButtonModel.SpecializationTypeId, (provider, componentTemplate) => new ButtonComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ContainerModel.SpecializationTypeId, (provider, componentTemplate) => new ContainerComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TableModel.SpecializationTypeId, (provider, componentTemplate) => new TableComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TextModel.SpecializationTypeId, (provider, componentTemplate) => new TextComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(LayoutModel.SpecializationTypeId, (provider, componentTemplate) => new LayoutComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(NavigationBarModel.SpecializationTypeId, (provider, componentTemplate) => new NavigationBarComponentBuilder(provider, componentTemplate));
        }
    }
}