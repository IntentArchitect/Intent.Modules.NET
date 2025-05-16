using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.UnitTesting.Templates.CommandHandlerTest;
using Intent.Modules.UnitTesting.Templates.OperationHandlerTest;
using Intent.Modules.UnitTesting.Templates.QueryHandlerTest;
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

        public static string GetOperationHandlerTestName<T>(this IIntentTemplate<T> template) where T : OperationModel
        {
            return template.GetTypeName(OperationHandlerTestTemplate.TemplateId, template.Model);
        }

        public static string GetOperationHandlerTestName(this IIntentTemplate template, OperationModel model)
        {
            return template.GetTypeName(OperationHandlerTestTemplate.TemplateId, model);
        }

        public static string GetQueryHandlerTestName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryHandlerTestTemplate.TemplateId, template.Model);
        }

        public static string GetQueryHandlerTestName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryHandlerTestTemplate.TemplateId, model);
        }

    }
}