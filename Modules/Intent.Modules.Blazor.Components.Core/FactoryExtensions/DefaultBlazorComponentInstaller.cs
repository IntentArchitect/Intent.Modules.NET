using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.Core.ComponentBuilders;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DefaultBlazorComponentInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Components.Core.DefaultBlazorComponentInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => -1;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            DefaultRazorComponentBuilderProvider.Register(FormModel.SpecializationTypeId, (provider, template) => new FormComponentBuilder(provider, template));
            DefaultRazorComponentBuilderProvider.Register(TextInputModel.SpecializationTypeId, (provider, template) => new TextInputComponentBuilder(provider, template));
            DefaultRazorComponentBuilderProvider.Register(ButtonModel.SpecializationTypeId, (provider, template) => new ButtonComponentBuilder(provider, template));
            DefaultRazorComponentBuilderProvider.Register(ContainerModel.SpecializationTypeId, (provider, template) => new ContainerComponentBuilder(provider, template));
            DefaultRazorComponentBuilderProvider.Register(TableModel.SpecializationTypeId, (provider, template) => new TableComponentBuilder(provider, template));
            DefaultRazorComponentBuilderProvider.Register(TextModel.SpecializationTypeId, (provider, template) => new TextComponentBuilder(provider, template));
            DefaultRazorComponentBuilderProvider.Register(DisplayComponentModel.SpecializationTypeId, (provider, template) => new CustomComponentBuilder(provider, template));
        }
    }
}