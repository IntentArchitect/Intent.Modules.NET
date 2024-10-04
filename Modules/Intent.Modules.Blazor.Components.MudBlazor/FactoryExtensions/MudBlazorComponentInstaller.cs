using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;
using Intent.Modules.Blazor.Components.MudBlazor.Interceptors;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MudBlazorComponentInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Components.MudBlazor.MudBlazorComponentInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            DefaultRazorComponentBuilderProvider.Register(ComponentViewModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentViewBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(FormModel.SpecializationTypeId, (provider, componentTemplate) => new FormComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TextInputModel.SpecializationTypeId, (provider, componentTemplate) => new TextInputComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ButtonModel.SpecializationTypeId, (provider, componentTemplate) => new ButtonComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ContainerModel.SpecializationTypeId, (provider, componentTemplate) => new ContainerComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TableModel.SpecializationTypeId, (provider, componentTemplate) => new TableComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TextModel.SpecializationTypeId, (provider, componentTemplate) => new TextComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(LayoutModel.SpecializationTypeId, (provider, componentTemplate) => new LayoutComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(NavigationMenuModel.SpecializationTypeId, (provider, componentTemplate) => new NavigationBarComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(IconModel.SpecializationTypeId, (provider, componentTemplate) => new IconComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(SelectModel.SpecializationTypeId, (provider, componentTemplate) => new SelectComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(CheckboxModel.SpecializationTypeId, (provider, componentTemplate) => new CheckboxComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(DataGridModel.SpecializationTypeId, (provider, componentTemplate) => new DataGridComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(DatePickerModel.SpecializationTypeId, (provider, componentTemplate) => new DatePickerComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(DialogModel.SpecializationTypeId, (provider, componentTemplate) => new DialogComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(CardModel.SpecializationTypeId, (provider, componentTemplate) => new CardComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(SpacerModel.SpecializationTypeId, (provider, componentTemplate) => new SpacerComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ImageModel.SpecializationTypeId, (provider, componentTemplate) => new ImageComponentBuilder(provider, componentTemplate));

            DefaultRazorComponentBuilderProvider.AddInterceptor((provider, componentTemplate) => new MudBlazorLayoutInterceptor(provider, componentTemplate));
        }
    }
}