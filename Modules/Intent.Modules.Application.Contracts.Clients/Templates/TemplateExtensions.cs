using System.Collections.Generic;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDtoContractName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyDTOModel
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, template.Model);
        }

        public static string GetDtoContractName(this IntentTemplateBase template, Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyDTOModel model)
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, model);
        }

        public static string GetServiceContractName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetServiceContractName(this IntentTemplateBase template, Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, model);
        }

    }
}