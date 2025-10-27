using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.AzureFunctions.AzureQueueStorage.Templates.AzureFunctionConsumer;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureQueueStorage.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureFunctionConsumerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(AzureFunctionConsumerTemplate.TemplateId, template.Model);
        }

        public static string GetAzureFunctionConsumerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(AzureFunctionConsumerTemplate.TemplateId, model);
        }

    }
}