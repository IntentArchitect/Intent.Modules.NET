using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TemplateSuppressorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Host.TemplateSuppressorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            DisableTemplate(application, "Intent.Eventing.MassTransit.IntegrationEventConsumer");
            //Entities (This module is only added for modeling Shared Enums basically)
            DisableTemplate(application, "Intent.Entities.CollectionExtensions");
            DisableTemplate(application, "Intent.Entities.UpdateHelper");
        }

        private void DisableTemplate(IApplication application, string templateId)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(templateId);

            if (template is not null)
            {
                template.CanRun = false;
            }

        }
        
    }
}