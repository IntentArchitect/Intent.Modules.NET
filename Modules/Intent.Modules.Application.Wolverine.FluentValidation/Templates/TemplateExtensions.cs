using System.Collections.Generic;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.Wolverine.FluentValidation.Templates.QueryValidator;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FluentValidation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCommandValidatorName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandValidatorTemplate.TemplateId, template.Model);
        }

        public static string GetCommandValidatorName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandValidatorTemplate.TemplateId, model);
        }

        public static string GetQueryValidatorName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryValidatorTemplate.TemplateId, template.Model);
        }

        public static string GetQueryValidatorName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryValidatorTemplate.TemplateId, model);
        }

    }
}