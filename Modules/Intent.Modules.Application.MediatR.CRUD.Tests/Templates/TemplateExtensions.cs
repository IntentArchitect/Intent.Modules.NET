using System.Collections.Generic;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.CreateAggregateRootCommandHandlerTests;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCreateAggregateRootCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(CreateAggregateRootCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetCreateAggregateRootCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(CreateAggregateRootCommandHandlerTestsTemplate.TemplateId, model);
        }

    }
}