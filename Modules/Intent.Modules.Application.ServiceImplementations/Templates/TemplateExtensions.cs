using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Templates
{
    public static class TemplateExtensions
    {
        public static string GetServiceImplementationName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(ServiceImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetServiceImplementationName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(ServiceImplementationTemplate.TemplateId, model);
        }

    }
}