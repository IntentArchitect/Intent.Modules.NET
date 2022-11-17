using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class SaveTransactionControllerDecorator : ControllerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.Interop.EntityFrameworkCore.SaveTransactionControllerDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public SaveTransactionControllerDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(GetUnitOfWork(), "unitOfWork", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<OperationModel>("model", out var operation) &&
                        operation.HasHttpSettings() && !operation.GetHttpSettings().Verb().IsGET())
                    {
                        _template.AddUsing("System.Transactions");
                        method.Statements.FirstOrDefault(x => x.ToString().Contains("await "))?
                            .InsertAbove($@"using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))
            {{")
                            .Indent()
                            .InsertBelow("await _unitOfWork.SaveChangesAsync(cancellationToken);", s => s
                            .InsertBelow("transaction.Complete();", s => s
                            .InsertBelow("}", s => s.Outdent())));
                    }
                }
            }, order: 1);
        }

        //public override int Priority => 100;

        //public override string EnterClass()
        //{
        //    return $@"
        //private readonly {GetUnitOfWork()} _unitOfWork;";
        //}

        //public override IEnumerable<string> ConstructorParameters()
        //{
        //    return new[] { $"{GetUnitOfWork()} unitOfWork" };
        //}

        //public override string ConstructorImplementation()
        //{
        //    return $@"
        //    _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));";
        //}

        //public override string EnterOperationBody(OperationModel operationModel)
        //{
        //    _template.AddUsing("System.Transactions");

        //    return $@"
        //    using (var transaction = new TransactionScope(TransactionScopeOption.Required,
        //        new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))
        //    {{";
        //}

        //public override string MidOperationBody(OperationModel operationModel)
        //{
        //    if (operationModel.GetHttpSettings().Verb().IsGET())
        //    {
        //        return string.Empty;
        //    }
        //    return $@"
        //    await _unitOfWork.SaveChangesAsync(cancellationToken);
        //    transaction.Complete();";
        //}

        private string GetUnitOfWork()
        {
            if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }
            throw new Exception($"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}