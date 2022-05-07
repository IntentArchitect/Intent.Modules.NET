using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DbContextSaveControllerDecorator : ControllerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.Interop.EntityFrameworkCore.DbContextSaveControllerDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public DbContextSaveControllerDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = 100;
        }

        public override string EnterClass()
        {
            return $@"
        private readonly {GetUnitOfWork()} _unitOfWork;";
        }

        public override IEnumerable<string> ConstructorParameters()
        {
            return new[] { $"{GetUnitOfWork()} unitOfWork" };
        }

        public override string ConstructorImplementation()
        {
            return $@"
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));";
        }

        public override string MidOperationBody(OperationModel operationModel)
        {
            if (operationModel.GetHttpSettings().Verb().IsGET())
            {
                return string.Empty;
            }
            return $@"
            await _unitOfWork.SaveChangesAsync(cancellationToken);";
        }

        private string GetUnitOfWork()
        {
            if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                _template.TryGetTypeName(DbContextTemplate.TemplateId, out unitOfWork))
            {
                return unitOfWork;
            }
            throw new Exception($"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}