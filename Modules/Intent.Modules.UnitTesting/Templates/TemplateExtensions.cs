using System.Collections.Generic;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.UnitTesting.Templates.CommandHandlerTest;
using Intent.Modules.UnitTesting.Templates.DomainEventHandlerTest;
using Intent.Modules.UnitTesting.Templates.IntegrationEventHandlerTest;
using Intent.Modules.UnitTesting.Templates.QueryHandlerTest;
using Intent.Modules.UnitTesting.Templates.ServiceOperationTest;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCommandHandlerTestName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandHandlerTestTemplate.TemplateId, template.Model);
        }

        public static string GetCommandHandlerTestName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandHandlerTestTemplate.TemplateId, model);
        }

        public static string GetDomainEventHandlerTestName<T>(this IIntentTemplate<T> template) where T : DomainEventHandlerModel
        {
            return template.GetTypeName(DomainEventHandlerTestTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEventHandlerTestName(this IIntentTemplate template, DomainEventHandlerModel model)
        {
            return template.GetTypeName(DomainEventHandlerTestTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventHandlerTestName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(IntegrationEventHandlerTestTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerTestName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerTestTemplate.TemplateId, model);
        }

        public static string GetQueryHandlerTestName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryHandlerTestTemplate.TemplateId, template.Model);
        }

        public static string GetQueryHandlerTestName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryHandlerTestTemplate.TemplateId, model);
        }

        public static string GetServiceOperationTestName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(ServiceOperationTestTemplate.TemplateId, template.Model);
        }

        public static string GetServiceOperationTestName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(ServiceOperationTestTemplate.TemplateId, model);
        }

    }
}