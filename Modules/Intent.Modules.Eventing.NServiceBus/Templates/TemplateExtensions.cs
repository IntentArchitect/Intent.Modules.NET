using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConfiguration;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusHostedService;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageBus;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageHandler;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates
{
    public static class TemplateExtensions
    {
        public static string GetNServiceBusConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(NServiceBusConfigurationTemplate.TemplateId);
        }

        public static string GetNServiceBusHostedServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(NServiceBusHostedServiceTemplate.TemplateId);
        }

        public static string GetNServiceBusMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(NServiceBusMessageBusTemplate.TemplateId);
        }

        public static string GetNServiceBusMessageHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(NServiceBusMessageHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetNServiceBusMessageHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(NServiceBusMessageHandlerTemplate.TemplateId, model);
        }

    }
}