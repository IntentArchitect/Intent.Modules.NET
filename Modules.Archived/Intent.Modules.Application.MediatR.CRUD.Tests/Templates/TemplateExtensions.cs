using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Extensions.RepositoryExtensions;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.FluentValidation.FluentValidationTest;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedCreateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedDeleteCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedGetAllQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedGetByIdQueryHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedUpdateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.CreateCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.DeleteCommandHandlerTests;
using Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Owner.GetAllPaginationQueryHandlerTests;
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
        public static string GetAssertionClassName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(AssertionClassTemplate.TemplateId, template.Model);
        }

        public static string GetAssertionClassName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(AssertionClassTemplate.TemplateId, model);
        }

        public static string GetRepositoryExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(RepositoryExtensionsTemplate.TemplateId);
        }

        public static string GetFluentValidationTestName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(FluentValidationTestTemplate.TemplateId, template.Model);
        }

        public static string GetFluentValidationTestName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(FluentValidationTestTemplate.TemplateId, model);
        }

        public static string GetNestedCreateCommandHandlerTestsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(NestedCreateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedCreateCommandHandlerTestsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(NestedCreateCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedDeleteCommandHandlerTestsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(NestedDeleteCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedDeleteCommandHandlerTestsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(NestedDeleteCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedGetAllQueryHandlerTestsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(NestedGetAllQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedGetAllQueryHandlerTestsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(NestedGetAllQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedGetByIdQueryHandlerTestsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(NestedGetByIdQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedGetByIdQueryHandlerTestsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(NestedGetByIdQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetNestedUpdateCommandHandlerTestsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(NestedUpdateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetNestedUpdateCommandHandlerTestsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(NestedUpdateCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetCreateCommandHandlerTestsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CreateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetCreateCommandHandlerTestsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CreateCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetDeleteCommandHandlerTestsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(DeleteCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetDeleteCommandHandlerTestsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(DeleteCommandHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetGetAllPaginationQueryHandlerTestsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(GetAllPaginationQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetGetAllPaginationQueryHandlerTestsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(GetAllPaginationQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetGetAllQueryHandlerTestsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(GetAllQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetGetAllQueryHandlerTestsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(GetAllQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetGetByIdQueryHandlerTestsName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(GetByIdQueryHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetGetByIdQueryHandlerTestsName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(GetByIdQueryHandlerTestsTemplate.TemplateId, model);
        }

        public static string GetUpdateCommandHandlerTestsName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(UpdateCommandHandlerTestsTemplate.TemplateId, template.Model);
        }

        public static string GetUpdateCommandHandlerTestsName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(UpdateCommandHandlerTestsTemplate.TemplateId, model);
        }

    }
}