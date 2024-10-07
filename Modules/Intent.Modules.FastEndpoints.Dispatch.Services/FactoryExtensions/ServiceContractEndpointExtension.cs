using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.Endpoint.Models;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Dispatch.Services.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceContractEndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.FastEndpoints.Dispatch.Services.ServiceContractEndpointExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var endpointTemplates = application.FindTemplateInstances<EndpointTemplate>(EndpointTemplate.TemplateId);
            foreach (var endpointTemplate in endpointTemplates)
            {
                if (endpointTemplate.Model is not ServiceEndpointModel)
                {
                    continue;
                }
                InstallServiceContractDispatch(endpointTemplate);
                InstallTransactionWithUnitOfWork(endpointTemplate);
                InstallMessageBus(endpointTemplate, application);
            }
        }

        private void InstallServiceContractDispatch(EndpointTemplate endpointTemplate)
        {
            endpointTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(endpointTemplate.GetTypeName(ServiceContractTemplate.TemplateId, endpointTemplate.Model.Container.InternalElement), "appService",
                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                var method = @class.FindMethod(s => s.HasMetadata("handle"))!;
                var serviceInvocation = new CSharpInvocationStatement($"_appService.{endpointTemplate.Model.Name.ToPascalCase()}");
                foreach (var parameter in endpointTemplate.Model.Parameters)
                {
                    if (parameter.TypeReference?.Element?.IsDTOModel() == true)
                    {
                        serviceInvocation.AddArgument($"req");
                    }
                    else
                    {
                        serviceInvocation.AddArgument($"req.{parameter.Name.ToPropertyName()}");
                    }
                }

                CSharpStatement invocation = serviceInvocation;
                if (!endpointTemplate.Model.InternalElement.HasStereotype("Synchronous"))
                {
                    serviceInvocation.AddArgument("ct");
                    invocation = new CSharpAwaitExpression(invocation);
                }

                if (endpointTemplate.Model.ReturnType is not null)
                {
                    invocation = new CSharpAssignmentStatement("result", invocation);

                    method.AddStatement($"var result = default({endpointTemplate.GetTypeName(endpointTemplate.Model.ReturnType)});");
                }

                invocation.AddMetadata("service-contract-dispatch", true);

                method.AddStatement(invocation);

                var returnStatement = endpointTemplate.GetReturnStatement();
                if (returnStatement is not null)
                {
                    method.AddStatement(returnStatement);
                }
            }, 2);
        }

        private void InstallTransactionWithUnitOfWork(EndpointTemplate endpointTemplate)
        {
            if (!endpointTemplate.SystemUsesPersistenceUnitOfWork())
            {
                return;
            }

            endpointTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod(s => s.HasMetadata("handle"))!;
                if (endpointTemplate.Model.Verb == HttpVerb.Get)
                {
                    return;
                }

                var dispatchStmt = method.Statements.FirstOrDefault(stmt => stmt.HasMetadata("service-contract-dispatch"));
                if (dispatchStmt == null)
                {
                    return;
                }

                //remove current dispatch statement (UOW implementation replaces it)
                dispatchStmt.Remove();
                method.ApplyUnitOfWorkImplementations(
                    template: endpointTemplate,
                    constructor: @class.Constructors.First(),
                    invocationStatement: dispatchStmt,
                    returnType: null,
                    resultVariableName: "result",
                    fieldSuffix: "unitOfWork",
                    includeComments: false,
                    cancellationTokenExpression: "ct");

                //Move return statement to the end
                var returnStatement = method.Statements.LastOrDefault(x => x.HasMetadata("response"));
                if (returnStatement != null)
                {
                    returnStatement.Remove();
                    method.AddStatement(returnStatement);
                }
            }, order: 3);
        }

        private static void InstallMessageBus(EndpointTemplate endpointTemplate, IApplication application)
        {
            if (application.FindTemplateInstance<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface) is null)
            {
                return;
            }

            endpointTemplate.CSharpFile.AfterBuild(file =>
            {
                if (endpointTemplate.Model.Verb == HttpVerb.Get)
                {
                    return;
                }

                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();

                ctor.AddParameter(endpointTemplate.GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface), "eventBus",
                    p => p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()));

                var method = @class.FindMethod(s => s.HasMetadata("handle"))!;
                method.Statements.LastOrDefault(x => x.HasMetadata("response"))
                    ?.InsertAbove("await _eventBus.FlushAllAsync(ct);", stmt => stmt.AddMetadata("eventbus-flush", true));
            }, order: -100);
        }
    }
}