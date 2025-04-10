using System.Collections.Generic;
using Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureFunctionConsumerName<T>(this IIntentTemplate<T> template) where T : AzureFunctionSubscriptionModel
        {
            return template.GetTypeName(AzureFunctionConsumerTemplate.TemplateId, template.Model);
        }

        public static string GetAzureFunctionConsumerName(this IIntentTemplate template, AzureFunctionSubscriptionModel model)
        {
            return template.GetTypeName(AzureFunctionConsumerTemplate.TemplateId, model);
        }

    }
}