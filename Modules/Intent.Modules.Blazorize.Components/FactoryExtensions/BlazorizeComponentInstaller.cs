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
    public class BlazorizeComponentInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazorize.Components.BlazorizeComponentInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            DefaultRazorComponentBuilderProvider.Register(FormModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentRenderer.FormComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TextInputModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentRenderer.TextInputComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ButtonModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentRenderer.ButtonComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(ContainerModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentRenderer.ContainerComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TableModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentRenderer.TableComponentBuilder(provider, componentTemplate));
            DefaultRazorComponentBuilderProvider.Register(TextModel.SpecializationTypeId, (provider, componentTemplate) => new ComponentRenderer.TextComponentBuilder(provider, componentTemplate));
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //var componentTemplates = application.FindTemplateInstances<RazorComponentTemplate>(RazorComponentTemplate.TemplateId);
            //foreach (var componentTemplate in componentTemplates)
            //{
            //    componentTemplate.ComponentBuilderResolver.Register(FormModel.SpecializationTypeId, new FormComponentBuilder(componentTemplate.ComponentBuilderResolver, componentTemplate));
            //    componentTemplate.ComponentBuilderResolver.Register(TextInputModel.SpecializationTypeId, new TextInputComponentBuilder(componentTemplate.ComponentBuilderResolver, componentTemplate));
            //    componentTemplate.ComponentBuilderResolver.Register(ButtonModel.SpecializationTypeId, new ButtonComponentBuilder(componentTemplate.ComponentBuilderResolver, componentTemplate));
            //    componentTemplate.ComponentBuilderResolver.Register(ContainerModel.SpecializationTypeId, new ContainerComponentBuilder(componentTemplate.ComponentBuilderResolver, componentTemplate));
            //    componentTemplate.ComponentBuilderResolver.Register(TableModel.SpecializationTypeId, new TableComponentBuilder(componentTemplate.ComponentBuilderResolver, componentTemplate));
            //    componentTemplate.ComponentBuilderResolver.Register(TextModel.SpecializationTypeId, new TextComponentBuilder(componentTemplate.ComponentBuilderResolver, componentTemplate));
            //}
        }
    }
}