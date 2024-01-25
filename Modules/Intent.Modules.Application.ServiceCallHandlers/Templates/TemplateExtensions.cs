using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandlerImplementation;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.ServiceCallHandlers.Templates
{
    public static class TemplateExtensions
    {
        public static string GetServiceCallHandlerImplementationName<T>(this IIntentTemplate<T> template) where T : OperationModel
        {
            return template.GetTypeName(ServiceCallHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetServiceCallHandlerImplementationName(this IIntentTemplate template, OperationModel model)
        {
            return template.GetTypeName(ServiceCallHandlerImplementationTemplate.TemplateId, model);
        }

    }
}