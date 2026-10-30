using System.Collections.Generic;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Mediator.Templates.AuthorizationBehaviour;
using Intent.Modules.Application.Mediator.Templates.CommandHandler;
using Intent.Modules.Application.Mediator.Templates.CommandModels;
using Intent.Modules.Application.Mediator.Templates.LoggingBehaviour;
using Intent.Modules.Application.Mediator.Templates.MediatorConfiguration;
using Intent.Modules.Application.Mediator.Templates.MessageBusFlushOwner;
using Intent.Modules.Application.Mediator.Templates.MessageBusPublishBehaviour;
using Intent.Modules.Application.Mediator.Templates.PerformanceBehaviour;
using Intent.Modules.Application.Mediator.Templates.QueryHandler;
using Intent.Modules.Application.Mediator.Templates.QueryModels;
using Intent.Modules.Application.Mediator.Templates.SequentialNotificationPublisher;
using Intent.Modules.Application.Mediator.Templates.UnhandledExceptionBehaviour;
using Intent.Modules.Application.Mediator.Templates.UnitOfWorkBehaviour;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAuthorizationBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthorizationBehaviourTemplate.TemplateId);
        }

        public static string GetCommandHandlerName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetCommandHandlerName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandHandlerTemplate.TemplateId, model);
        }

        public static string GetCommandModelsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, template.Model);
        }

        public static string GetCommandModelsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, model);
        }

        public static string GetLoggingBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(LoggingBehaviourTemplate.TemplateId);
        }

        public static string GetMediatorConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(MediatorConfigurationTemplate.TemplateId);
        }

        public static string GetMessageBusFlushOwnerName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageBusFlushOwnerTemplate.TemplateId);
        }

        public static string GetMessageBusPublishBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageBusPublishBehaviourTemplate.TemplateId);
        }

        public static string GetPerformanceBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(PerformanceBehaviourTemplate.TemplateId);
        }

        public static string GetQueryHandlerName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetQueryHandlerName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryHandlerTemplate.TemplateId, model);
        }

        public static string GetQueryModelsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, template.Model);
        }

        public static string GetQueryModelsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, model);
        }

        public static string GetSequentialNotificationPublisherName(this IIntentTemplate template)
        {
            return template.GetTypeName(SequentialNotificationPublisherTemplate.TemplateId);
        }

        public static string GetUnhandledExceptionBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(UnhandledExceptionBehaviourTemplate.TemplateId);
        }

        public static string GetUnitOfWorkBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(UnitOfWorkBehaviourTemplate.TemplateId);
        }

    }
}