using System.Collections.Generic;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.QueryValidator;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates
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

        public static string GetValidationBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValidationBehaviourTemplate.TemplateId);
        }

    }
}