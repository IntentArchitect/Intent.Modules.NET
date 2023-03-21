using System.Collections.Generic;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Extensions.RepositoryExtensions;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedCreateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedDeleteCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedGetAllQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedGetByIdQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedUpdateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.CreateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.DeleteCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.GetAllQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.GetByIdQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.UpdateCommandHandlerTests;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAssertionClassName<T>(this IntentTemplateBase<T> template)
where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(AssertionClassTemplate.TemplateId, template.Model);
        }

        public static string GetAssertionClassName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(AssertionClassTemplate.TemplateId, model);
        }
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

        public static string GetNestedCreateCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(NestedCreateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedCreateCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(NestedCreateCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedDeleteCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(NestedDeleteCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedDeleteCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(NestedDeleteCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedGetAllQueryHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.QueryModel
        {
            return template.GetTypeName(NestedGetAllQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedGetAllQueryHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.QueryModel model)
        {
            return template.GetTypeName(NestedGetAllQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedGetByIdQueryHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.QueryModel
        {
            return template.GetTypeName(NestedGetByIdQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedGetByIdQueryHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.QueryModel model)
        {
            return template.GetTypeName(NestedGetByIdQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedUpdateCommandHandlerTestsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.CQRS.Api.CommandModel
        {
            return template.GetTypeName(NestedUpdateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedUpdateCommandHandlerTestsName(this IntentTemplateBase template, Intent.Modelers.Services.CQRS.Api.CommandModel model)
        {
            return template.GetTypeName(NestedUpdateCommandHandlerTestsTemplate.TemplateId, model);
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