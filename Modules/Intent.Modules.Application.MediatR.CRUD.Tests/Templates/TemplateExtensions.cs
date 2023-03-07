using System.Collections.Generic;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.CreateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.DeleteCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.GetAllQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.GetByIdQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.RepositoryExtensions;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.UpdateCommandHandlerTests;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCreateCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(CreateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetCreateCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(CreateCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetDeleteCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(DeleteCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetDeleteCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(DeleteCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetGetAllQueryHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.QueryModel
        {
            return template.GetTypeName(GetAllQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetGetAllQueryHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.QueryModel model)
        {
            return template.GetTypeName(GetAllQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetGetByIdQueryHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.QueryModel
        {
            return template.GetTypeName(GetByIdQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetGetByIdQueryHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.QueryModel model)
        {
            return template.GetTypeName(GetByIdQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetRepositoryExtensionsName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(RepositoryExtensionsTemplate.TemplateId);
        }

        public static string GetUpdateCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(UpdateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetUpdateCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(UpdateCommandHandlerTestsTemplate.TemplateId, model);
        }

    }
}