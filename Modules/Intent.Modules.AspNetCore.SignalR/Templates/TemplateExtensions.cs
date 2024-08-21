using System.Collections.Generic;
using Intent.AspNetCore.SignalR.Api;
using Intent.Modules.AspNetCore.SignalR.Templates.Hub;
using Intent.Modules.AspNetCore.SignalR.Templates.HubService;
using Intent.Modules.AspNetCore.SignalR.Templates.HubServiceInterface;
using Intent.Modules.AspNetCore.SignalR.Templates.SignalRConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHubName<T>(this IIntentTemplate<T> template) where T : SignalRHubModel
        {
            return template.GetTypeName(HubTemplate.TemplateId, template.Model);
        }

        public static string GetHubName(this IIntentTemplate template, SignalRHubModel model)
        {
            return template.GetTypeName(HubTemplate.TemplateId, model);
        }

        public static string GetHubServiceName<T>(this IIntentTemplate<T> template) where T : SignalRHubModel
        {
            return template.GetTypeName(HubServiceTemplate.TemplateId, template.Model);
        }

        public static string GetHubServiceName(this IIntentTemplate template, SignalRHubModel model)
        {
            return template.GetTypeName(HubServiceTemplate.TemplateId, model);
        }

        public static string GetHubServiceInterfaceName<T>(this IIntentTemplate<T> template) where T : SignalRHubModel
        {
            return template.GetTypeName(HubServiceInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetHubServiceInterfaceName(this IIntentTemplate template, SignalRHubModel model)
        {
            return template.GetTypeName(HubServiceInterfaceTemplate.TemplateId, model);
        }

        public static string GetSignalRConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(SignalRConfigurationTemplate.TemplateId);
        }

    }
}