using System.Collections.Generic;
using Intent.Modules.Application.MediatR.Behaviours.Templates.AuthorizationBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.EventBusPublishBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.LoggingBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.MongoDbUnitOfWorkBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.PerformanceBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.UnhandledExceptionBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.UnitOfWorkBehaviour;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAuthorizationBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AuthorizationBehaviourTemplate.TemplateId);
        }

        public static string GetEventBusPublishBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(EventBusPublishBehaviourTemplate.TemplateId);
        }

        public static string GetLoggingBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(LoggingBehaviourTemplate.TemplateId);
        }

        public static string GetMongoDbUnitOfWorkBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MongoDbUnitOfWorkBehaviourTemplate.TemplateId);
        }

        public static string GetPerformanceBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(PerformanceBehaviourTemplate.TemplateId);
        }

        public static string GetUnhandledExceptionBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UnhandledExceptionBehaviourTemplate.TemplateId);
        }

        public static string GetUnitOfWorkBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UnitOfWorkBehaviourTemplate.TemplateId);
        }

    }
}