using System.Collections.Generic;
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
        public static string GetServiceImplementationName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.ServiceModel
        {
            return template.GetTypeName(ServiceImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetServiceImplementationName(this IntentTemplateBase template, Intent.Modelers.Services.Api.ServiceModel model)
        {
            return template.GetTypeName(ServiceImplementationTemplate.TemplateId, model);
        }

    }
}