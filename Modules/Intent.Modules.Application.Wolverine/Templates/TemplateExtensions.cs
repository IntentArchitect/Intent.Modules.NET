using System.Collections.Generic;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates.ApplicationHandlerPolicy;
using Intent.Modules.Application.Wolverine.Templates.AuthorizationMiddleware;
using Intent.Modules.Application.Wolverine.Templates.CommandHandler;
using Intent.Modules.Application.Wolverine.Templates.CommandInterface;
using Intent.Modules.Application.Wolverine.Templates.CommandModels;
using Intent.Modules.Application.Wolverine.Templates.LoggingMiddleware;
using Intent.Modules.Application.Wolverine.Templates.PerformanceMiddleware;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
using Intent.Modules.Application.Wolverine.Templates.QueryInterface;
using Intent.Modules.Application.Wolverine.Templates.QueryModels;
using Intent.Modules.Application.Wolverine.Templates.UnhandledExceptionMiddleware;
using Intent.Modules.Application.Wolverine.Templates.UnitOfWorkMiddleware;
using Intent.Modules.Application.Wolverine.Templates.ValidationMiddleware;
using Intent.Modules.Application.Wolverine.Templates.WolverineConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates
{
    public static class TemplateExtensions
    {
        public static string GetApplicationHandlerPolicyName(this IIntentTemplate template)
        {
            return template.GetTypeName(ApplicationHandlerPolicyTemplate.TemplateId);
        }
        public static string GetAuthorizationMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthorizationMiddlewareTemplate.TemplateId);
        }

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

        public static string GetLoggingMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(LoggingMiddlewareTemplate.TemplateId);
        }

        public static string GetPerformanceMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(PerformanceMiddlewareTemplate.TemplateId);
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

        public static string GetUnhandledExceptionMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(UnhandledExceptionMiddlewareTemplate.TemplateId);
        }

        public static string GetUnitOfWorkMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(UnitOfWorkMiddlewareTemplate.TemplateId);
        }

        public static string GetValidationMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(ValidationMiddlewareTemplate.TemplateId);
        }

        public static string GetWolverineConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(WolverineConfigurationTemplate.TemplateId);
        }

    }
}