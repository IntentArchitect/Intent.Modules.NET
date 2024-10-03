using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
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

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var endpointTemplates = application.FindTemplateInstances<EndpointTemplate>(EndpointTemplate.TemplateId);
            foreach (var endpointTemplate in endpointTemplates)
            {
                InstallServiceContractDispatch(endpointTemplate);
            }
        }

        private void InstallServiceContractDispatch(EndpointTemplate endpointTemplate)
        {
            endpointTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(endpointTemplate.GetTypeName(ServiceContractTemplate.TemplateId, endpointTemplate.Model.Container.InternalElement), "appService", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                var method = @class.FindMethod(s => s.TryGetMetadata("handle", out _))!;
                var serviceInvocation = new CSharpInvocationStatement($"_appService.{endpointTemplate.Model.Name.ToPascalCase()}");
                foreach (var parameter in endpointTemplate.Model.Parameters)
                {
                    serviceInvocation.AddArgument($"req.{parameter.Name.ToPropertyName()}");
                }
                serviceInvocation.AddMetadata("service-contract-dispatch", true);

                CSharpStatement invocation = serviceInvocation;
                if (!endpointTemplate.Model.InternalElement.HasStereotype("Synchronous"))
                {
                    serviceInvocation.AddArgument("ct");
                    invocation = new CSharpAwaitExpression(invocation);
                }

                if (endpointTemplate.Model.ReturnType is not null)
                {
                    invocation = new CSharpAssignmentStatement("var result", invocation);
                }

                method.AddStatement(invocation);
                
                var returnStatement = endpointTemplate.GetReturnStatement();
                if (returnStatement is not null)
                {
                    method.AddStatement(returnStatement);
                }
            });
        }
    }
}