using System;
using System.Collections.Generic;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DbContextSaveFunctionDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.AzureFunctions.Interop.EntityFrameworkCore.DbContextSaveFunctionDecorator";

        [IntentManaged(Mode.Fully)] private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbContextSaveFunctionDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = 100;
        }

        public override IEnumerable<string> GetClassEntryDefinitionList()
        {
            if (_template.Model.GetAzureFunction()?.GetHttpTriggerView()?.Method().IsGET() == true)
            {
                yield break;
            }

            yield return $"private readonly {GetUnitOfWork()} _unitOfWork;";
        }

        public override IEnumerable<string> GetConstructorParameterDefinitionList()
        {
            if (_template.Model.GetAzureFunction()?.GetHttpTriggerView()?.Method().IsGET() == true)
            {
                yield break;
            }

            yield return $"{GetUnitOfWork()} unitOfWork";
        }

        public override IEnumerable<string> GetConstructorBodyStatementList()
        {
            if (_template.Model.GetAzureFunction()?.GetHttpTriggerView()?.Method().IsGET() == true)
            {
                yield break;
            }

            yield return $"_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));";
        }

        public override IEnumerable<string> GetRunMethodBodyStatementList()
        {
            if (_template.Model.GetAzureFunction()?.GetHttpTriggerView()?.Method().IsGET() == true)
            {
                yield break;
            }

            yield return $"await _unitOfWork.SaveChangesAsync();";
        }

        private string GetUnitOfWork()
        {
            if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface,
                    out unitOfWork) ||
                _template.TryGetTypeName(DbContextTemplate.TemplateId, out unitOfWork))
            {
                return unitOfWork;
            }

            throw new Exception(
                $"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}