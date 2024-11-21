using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Mvc.Templates.MvcController;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Mvc.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMvcControllerName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(MvcControllerTemplate.TemplateId, template.Model);
        }

        public static string GetMvcControllerName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(MvcControllerTemplate.TemplateId, model);
        }

    }
}