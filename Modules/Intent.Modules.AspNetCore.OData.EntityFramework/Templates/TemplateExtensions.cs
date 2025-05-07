using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AspNetCore.OData.EntityFramework.Templates.ODataAggregateController;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OData.EntityFramework.Templates
{
    public static class TemplateExtensions
    {
        public static string GetODataAggregateControllerName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(ODataAggregateControllerTemplate.TemplateId, template.Model);
        }

        public static string GetODataAggregateControllerName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(ODataAggregateControllerTemplate.TemplateId, model);
        }

    }
}