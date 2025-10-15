using Intent.Modules.Aws.Sqs.Templates.SqsConfiguration;
using Intent.Modules.Aws.Sqs.Templates.SqsEventBus;
using Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcher;
using Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcherInterface;
using Intent.Modules.Aws.Sqs.Templates.SqsPublisherOptions;
using Intent.Modules.Aws.Sqs.Templates.SqsSubscriptionOptions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs.Templates
{
    public static class TemplateExtensions
    {
        public static string GetSqsConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(SqsConfigurationTemplate.TemplateId);
        }

        public static string GetSqsEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(SqsEventBusTemplate.TemplateId);
        }

        public static string GetSqsMessageDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(SqsMessageDispatcherTemplate.TemplateId);
        }

        public static string GetSqsMessageDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(SqsMessageDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetSqsPublisherOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(SqsPublisherOptionsTemplate.TemplateId);
        }

        public static string GetSqsSubscriptionOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(SqsSubscriptionOptionsTemplate.TemplateId);
        }
    }
}
