using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.BinaryContentAttribute;
using Intent.Modules.AspNetCore.Controllers.Templates.BinaryContentFilter;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.ExceptionFilter;
using Intent.Modules.AspNetCore.Controllers.Templates.JsonResponse;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates
{
    public static class TemplateExtensions
    {
        public static string GetBinaryContentAttributeName(this IIntentTemplate template)
        {
            return template.GetTypeName(BinaryContentAttributeTemplate.TemplateId);
        }

        public static string GetBinaryContentFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(BinaryContentFilterTemplate.TemplateId);
        }
        public static string GetControllerName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(ControllerTemplate.TemplateId, template.Model);
        }

        public static string GetControllerName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(ControllerTemplate.TemplateId, model);
        }

        public static string GetExceptionFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(ExceptionFilterTemplate.TemplateId);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetJsonResponseName<T>(this IntentTemplateBase<T> template)
        {
            var jsonResponseTemplate = template.GetTemplate<JsonResponseTemplate>(JsonResponseTemplate.TemplateId);
            jsonResponseTemplate.NotifyTemplateIsRequired(); // GCB - consider a way to track which templates resolve this one and can use that to determine if is required.
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }

    }
}