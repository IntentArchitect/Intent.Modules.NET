using System.Collections.Generic;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Templates
{
    public static class TemplateExtensions
    {
        public static string GetServiceContractName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Services.Api.ServiceModel
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetServiceContractName(this IIntentTemplate template, Intent.Modelers.Services.Api.ServiceModel model)
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, model);
        }

    }
}