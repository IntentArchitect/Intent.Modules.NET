using System.Collections.Generic;
using Intent.Modules.Application.MediatR.Behaviours.Templates.AuthorizationBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.LoggingBehaviour;
using Intent.Modules.Application.MediatR.Behaviours.Templates.MessageBusPublishBehaviour;
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

        public static string GetLoggingBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(LoggingBehaviourTemplate.TemplateId);
        }

        public static string GetMessageBusPublishBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageBusPublishBehaviourTemplate.TemplateId);
        }

        public static string GetPerformanceBehaviourName(this IIntentTemplate template)
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