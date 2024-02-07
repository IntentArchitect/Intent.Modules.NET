using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AzureFunctionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Interop.EntityFrameworkCore.AzureFunctionExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<AzureFunctionClassTemplate>(TemplateDependency.OnTemplate(AzureFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                template.AddTypeSource("Domain.NotFoundException");

                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var runMethod = @class.FindMethod("Run");

                    runMethod.FindStatement<CSharpTryBlock>(x => true)
                        ?.InsertBelow(new CSharpCatchBlock(template.GetTypeName("Domain.NotFoundException"),
                                "exception")
                            .AddStatement("return new NotFoundObjectResult(new { Message = exception.Message });"));
                });

                if (HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement)?.Verb == HttpVerb.Get)
                {
                    continue;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System.Transactions");
                    var @class = file.Classes.First();
                    @class.Constructors.First().AddParameter(GetUnitOfWork(template), "unitOfWork",
                        param => { param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                    var runMethod = @class.FindMethod("Run");
                    var dispatchStatement = runMethod.FindStatement(x => x.HasMetadata("service-dispatch-statement"));

                    if (dispatchStatement is null) return;
                    var returnStatement = runMethod.FindStatement(x => x.HasMetadata("return"));

                    var transactionScope = new CSharpUsingBlock($@"var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled)")
                        .AddStatement($"await _unitOfWork.SaveChangesAsync(cancellationToken);")
                        .AddStatement("transaction.Complete();");
                    transactionScope.AddMetadata("transaction-scope", true);
                    dispatchStatement.InsertAbove(transactionScope);
                    dispatchStatement.Remove();

                    transactionScope.InsertStatement(0, dispatchStatement);
                    if (returnStatement is not null)
                    {
                        returnStatement.Remove();
                        transactionScope.InsertStatement(transactionScope.Statements.Count, returnStatement);
                    }
                }, 150);
            }
        }

        private static string GetUnitOfWork(AzureFunctionClassTemplate template)
        {
            if (template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out var unitOfWork) ||
                template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                template.TryGetTypeName(TemplateRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }

            throw new Exception(
                $"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateRoles.Domain.UnitOfWork}] exists.");
        }
    }
}