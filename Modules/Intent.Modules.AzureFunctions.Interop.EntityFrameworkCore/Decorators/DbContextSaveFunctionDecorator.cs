using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DbContextSaveFunctionDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)] 
        public const string DecoratorId = "Intent.AzureFunctions.Interop.EntityFrameworkCore.DbContextSaveFunctionDecorator";

        [IntentManaged(Mode.Fully)] 
        private readonly AzureFunctionClassTemplate _template;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbContextSaveFunctionDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            if (_template.Model.GetAzureFunction()?.GetHttpTriggerView()?.Method().IsGET() == true ||
                template.Model.Mapping?.Element?.AsOperationModel() == null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("FluentValidation");
                var @class = file.Classes.Single();
                @class.Constructors.First().AddParameter(GetUnitOfWork(), "unitOfWork", param =>
                {
                    param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                var runMethod = @class.FindMethod("Run");
                
                
                runMethod.FindStatement(x => x.HasMetadata("service-dispatch-statement"))
                    ?.InsertBelow($"await _unitOfWork.SaveChangesAsync();");
            }, 10);
        }

        private string GetUnitOfWork()
        {
            if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }

            throw new Exception(
                $"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}