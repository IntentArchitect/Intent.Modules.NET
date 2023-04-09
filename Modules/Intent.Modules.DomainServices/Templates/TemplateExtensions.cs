using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainServices.Templates.DomainServiceImplementation;
using Intent.Modules.DomainServices.Templates.DomainServiceInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.DomainServices.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDomainServiceImplementationName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Services.Api.DomainServiceModel
        {
            return template.GetTypeName(DomainServiceImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetDomainServiceImplementationName(this IIntentTemplate template, Intent.Modelers.Domain.Services.Api.DomainServiceModel model)
        {
            return template.GetTypeName(DomainServiceImplementationTemplate.TemplateId, model);
        }

        public static string GetDomainServiceInterfaceName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Services.Api.DomainServiceModel
        {
            return template.GetTypeName(DomainServiceInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetDomainServiceInterfaceName(this IIntentTemplate template, Intent.Modelers.Domain.Services.Api.DomainServiceModel model)
        {
            return template.GetTypeName(DomainServiceInterfaceTemplate.TemplateId, model);
        }

    }
}