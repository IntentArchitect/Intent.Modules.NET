using System.Collections.Generic;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.JsonResponse;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates
{
    public static class TemplateExtensions
    {
        public static string GetControllerName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.ServiceModel
        {
            return template.GetTypeName(ControllerTemplate.TemplateId, template.Model);
        }

        public static string GetControllerName(this IntentTemplateBase template, Intent.Modelers.Services.Api.ServiceModel model)
        {
            return template.GetTypeName(ControllerTemplate.TemplateId, model);
        }

        public static string GetJsonResponseName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }

    }
}