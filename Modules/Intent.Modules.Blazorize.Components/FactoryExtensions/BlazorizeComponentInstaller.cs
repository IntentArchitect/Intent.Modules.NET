using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
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
            var componentTemplates = application.FindTemplateInstances<RazorComponentTemplate>(RazorComponentTemplate.TemplateId);
            foreach (var componentTemplate in componentTemplates)
            {
                componentTemplate.ComponentBuilderResolver.Register(FormModel.SpecializationTypeId, new FormComponentRenderer(componentTemplate.ComponentBuilderResolver, componentTemplate));
                componentTemplate.ComponentBuilderResolver.Register(TextInputModel.SpecializationTypeId, new TextInputComponent(componentTemplate.ComponentBuilderResolver, componentTemplate));
                componentTemplate.ComponentBuilderResolver.Register(ButtonModel.SpecializationTypeId, new ButtonRenderer(componentTemplate.ComponentBuilderResolver, componentTemplate));
                componentTemplate.ComponentBuilderResolver.Register(ContainerModel.SpecializationTypeId, new ContainerRenderer(componentTemplate.ComponentBuilderResolver, componentTemplate));
                componentTemplate.ComponentBuilderResolver.Register(TableModel.SpecializationTypeId, new TableRenderer(componentTemplate.ComponentBuilderResolver, componentTemplate));
                componentTemplate.ComponentBuilderResolver.Register(TextModel.SpecializationTypeId, new TextRenderer(componentTemplate.ComponentBuilderResolver, componentTemplate));
            }
        }
    }
}