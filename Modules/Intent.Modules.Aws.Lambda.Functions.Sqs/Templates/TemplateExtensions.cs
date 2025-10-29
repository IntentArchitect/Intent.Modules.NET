using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Aws.Lambda.Functions.Sqs.Templates.LambdaFunctionConsumer;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Sqs.Templates
{
    public static class TemplateExtensions
    {
        public static string GetLambdaFunctionConsumerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(LambdaFunctionConsumerTemplate.TemplateId, template.Model);
        }

        public static string GetLambdaFunctionConsumerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(LambdaFunctionConsumerTemplate.TemplateId, model);
        }

    }
}