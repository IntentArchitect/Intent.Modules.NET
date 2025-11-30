using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Solace.Templates.BaseMessage;
using Intent.Modules.Eventing.Solace.Templates.BaseMessageConverter;
using Intent.Modules.Eventing.Solace.Templates.DispatchResolver;
using Intent.Modules.Eventing.Solace.Templates.MessageRegistry;
using Intent.Modules.Eventing.Solace.Templates.MessageSerializer;
using Intent.Modules.Eventing.Solace.Templates.SolaceConfiguration;
using Intent.Modules.Eventing.Solace.Templates.SolaceConsumer;
using Intent.Modules.Eventing.Solace.Templates.SolaceConsumingService;
using Intent.Modules.Eventing.Solace.Templates.SolaceEventDispatcher;
using Intent.Modules.Eventing.Solace.Templates.SolaceEventDispatcherInterface;
using Intent.Modules.Eventing.Solace.Templates.SolaceMessageBus;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates
{
    public static class TemplateExtensions
    {
        public static string GetBaseMessageName(this IIntentTemplate template)
        {
            return template.GetTypeName(BaseMessageTemplate.TemplateId);
        }
        public static string GetBaseMessageConverterName(this IIntentTemplate template)
        {
            return template.GetTypeName(BaseMessageConverterTemplate.TemplateId);
        }

        public static string GetDispatchResolverName(this IIntentTemplate template)
        {
            return template.GetTypeName(DispatchResolverTemplate.TemplateId);
        }

        public static string GetMessageRegistryName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageRegistryTemplate.TemplateId);
        }

        public static string GetMessageSerializerName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageSerializerTemplate.TemplateId);
        }

        public static string GetSolaceConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(SolaceConfigurationTemplate.TemplateId);
        }

        public static string GetSolaceConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(SolaceConsumerTemplate.TemplateId);
        }

        public static string GetSolaceConsumingServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(SolaceConsumingServiceTemplate.TemplateId);
        }

        public static string GetSolaceEventDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(SolaceEventDispatcherTemplate.TemplateId);
        }

        public static string GetSolaceEventDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(SolaceEventDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetSolaceMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(SolaceMessageBusTemplate.TemplateId);
        }

    }
}