using System.Collections.Generic;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandInterface;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Application.MediatR.Templates.QueryInterface;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCommandHandlerName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetCommandHandlerName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandHandlerTemplate.TemplateId, model);
        }

        public static string GetCommandInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CommandInterfaceTemplate.TemplateId);
        }

        public static string GetCommandModelsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, template.Model);
        }

        public static string GetCommandModelsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, model);
        }

        public static string GetQueryHandlerName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetQueryHandlerName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryHandlerTemplate.TemplateId, model);
        }

        public static string GetQueryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(QueryInterfaceTemplate.TemplateId);
        }

        public static string GetQueryModelsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, template.Model);
        }

        public static string GetQueryModelsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetCommandHandlerName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(CommandHandlerTemplate.TemplateId, template.Model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetCommandHandlerName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(CommandHandlerTemplate.TemplateId, model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetCommandInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CommandInterfaceTemplate.TemplateId);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetCommandModelsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, template.Model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetCommandModelsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetQueryHandlerName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.QueryModel
        {
            return template.GetTypeName(QueryHandlerTemplate.TemplateId, template.Model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetQueryHandlerName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.QueryModel model)
        {
            return template.GetTypeName(QueryHandlerTemplate.TemplateId, model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetQueryInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(QueryInterfaceTemplate.TemplateId);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetQueryModelsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.QueryModel
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, template.Model);
        }

        [IntentManaged(Mode.Ignore)]
        public static string GetQueryModelsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.QueryModel model)
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, model);
        }

    }
}