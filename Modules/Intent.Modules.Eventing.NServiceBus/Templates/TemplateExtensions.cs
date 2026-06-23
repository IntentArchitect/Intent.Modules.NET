using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConfiguration;
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

        public static string GetNServiceBusMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(NServiceBusMessageBusTemplate.TemplateId);
        }

        public static string GetNServiceBusMessageHandlerName(this IIntentTemplate template)
        {
            return template.GetTypeName(NServiceBusMessageHandlerTemplate.TemplateId);
        }

    }
}