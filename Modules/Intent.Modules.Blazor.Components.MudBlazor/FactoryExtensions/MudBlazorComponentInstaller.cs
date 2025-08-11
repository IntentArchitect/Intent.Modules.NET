using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;
using Intent.Modules.Blazor.Components.MudBlazor.Interceptors;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
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
            DefaultRazorComponentBuilderProvider.Register(LinkModel.SpecializationTypeId, (provider, componentTemplate) => new LinkComponentBuilder(provider, componentTemplate));
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
            DefaultRazorComponentBuilderProvider.Register(AutoCompleteModel.SpecializationTypeId, (provider, componentTemplate) => new AutoCompleteComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(RadioGroupModel.SpecializationTypeId, (provider, componentTemplate) => new RadioGroupComponentBuilder(provider, componentTemplate));

            DefaultRazorComponentBuilderProvider.AddInterceptor((provider, componentTemplate) => new MudBlazorLayoutInterceptor(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.AddInterceptor((provider, componentTemplate) => new AuthorizedInterceptor(componentTemplate));

            application.ConfigureRazorTagMatchingFor("CardHeaderContent", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("CellTemplate", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("Columns", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("DialogActions", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("HeaderContent", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudAutocomplete", c => c.AllowMatchByAttributes("Label"));
            application.ConfigureRazorTagMatchingFor("MudButton", c => c.AllowMatchByAttributes("Href", "OnClick"));
            application.ConfigureRazorTagMatchingFor("MudCardActions", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudCardContent", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudCardHeader", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudDataGrid", c => c.AllowMatchByAttributes("@ref", "Items"));
            application.ConfigureRazorTagMatchingFor("MudDataGridPager", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudDatePicker", c => c.AllowMatchByAttributes("@bind-Date"));
            application.ConfigureRazorTagMatchingFor("MudDialogProvider", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudDrawer", c => c.AllowMatchByNameOnly()); // Although it seems possible to have more than one, I am guessing it's highly unusual.
            application.ConfigureRazorTagMatchingFor("MudGrid", c => c.AllowMatchByDescendant(["MudItem"]));
            application.ConfigureRazorTagMatchingFor("MudIconButton", c => c.AllowMatchByAttributes("Href"));
            application.ConfigureRazorTagMatchingFor("MudImage", c => c.AllowMatchByAttributes("Src"));
            application.ConfigureRazorTagMatchingFor("MudLayout", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudMainContent", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudMenu", c => c.AllowMatchByAttributes("Label"));
            application.ConfigureRazorTagMatchingFor("MudMenuItem", c => c.AllowMatchByAttributes("Href", "OnClick"));
            application.ConfigureRazorTagMatchingFor("MudNavGroup", c => c.AllowMatchByAttributes("href", "title"));
            application.ConfigureRazorTagMatchingFor("MudNavLink", c => c.AllowMatchByAttributes("href"));
            application.ConfigureRazorTagMatchingFor("MudNavMenu", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudPagination", c => c.AllowMatchByAttributes("@bind-Selected"));
            application.ConfigureRazorTagMatchingFor("MudPopoverProvider", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudRadioGroup", c => c.AllowMatchByAttributes("Label"));
            application.ConfigureRazorTagMatchingFor("MudSelect", c => c.AllowMatchByAttributes("Label"));
            application.ConfigureRazorTagMatchingFor("MudSnackbarProvider", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("MudThemeProvider", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("PagerContent", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("RowTemplate", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("TitleContent", c => c.AllowMatchByNameOnly());
            application.ConfigureRazorTagMatchingFor("ToolBarContent", c => c.AllowMatchByNameOnly());
        }
    }
}