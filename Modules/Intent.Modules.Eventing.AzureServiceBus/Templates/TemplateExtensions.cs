using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusConfiguration;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusHostedService;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageBus;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageDispatcher;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageDispatcherInterface;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusPublisherOptions;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusSubscriptionOptions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureServiceBusConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusConfigurationTemplate.TemplateId);
        }

        public static string GetAzureServiceBusHostedServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusHostedServiceTemplate.TemplateId);
        }

        public static string GetAzureServiceBusMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusMessageBusTemplate.TemplateId);
        }

        public static string GetAzureServiceBusMessageDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusMessageDispatcherTemplate.TemplateId);
        }

        public static string GetAzureServiceBusMessageDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusMessageDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetAzureServiceBusPublisherOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusPublisherOptionsTemplate.TemplateId);
        }

        public static string GetAzureServiceBusSubscriptionOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusSubscriptionOptionsTemplate.TemplateId);
        }

        [IntentIgnore]
        public static string GetSubscriptionEntryName<T>(this Intent.Modules.Common.CSharp.Templates.CSharpTemplateBase<T> template)
        {
            return template.TryGetTemplate<Intent.Modules.Common.CSharp.Templates.ICSharpTemplate>(AzureServiceBusSubscriptionOptionsTemplate.TemplateId, out var t)
                ? template.NormalizeNamespace($"{t.Namespace}.{AzureServiceBusSubscriptionOptionsTemplate.SubscriptionEntry}")
                : throw new System.InvalidOperationException();
        }
    }
}