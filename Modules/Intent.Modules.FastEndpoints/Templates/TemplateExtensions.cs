using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.ExceptionProcessor;
using Intent.Modules.FastEndpoints.Templates.JsonResponse;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates
{
    public static class TemplateExtensions
    {
        public static string GetEndpointTemplateName<T>(this IIntentTemplate<T> template) where T : IEndpointModel
        {
            return template.GetTypeName(EndpointTemplate.TemplateId, template.Model);
        }

        public static string GetEndpointTemplateName(this IIntentTemplate template, IEndpointModel model)
        {
            return template.GetTypeName(EndpointTemplate.TemplateId, model);
        }

        public static string GetExceptionProcessorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ExceptionProcessorTemplate.TemplateId);
        }

        public static string GetJsonResponseTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }

    }
}