using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.FastEndpoints.Dispatch.MediatR.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Dispatch.MediatR.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MediatREndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.FastEndpoints.Dispatch.MediatR.MediatREndpointExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var endpointTemplates = application.FindTemplateInstances<EndpointTemplate>(EndpointTemplate.TemplateId);
            foreach (var endpointTemplate in endpointTemplates)
            {
                if (endpointTemplate.Model is not MediatREndpointModel)
                {
                    continue;
                }
                InstallMediatRDispatch(endpointTemplate);
            }
        }

        private void InstallMediatRDispatch(EndpointTemplate endpointTemplate)
        {
            endpointTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(endpointTemplate.UseType("MediatR.ISender"), "mediator",
                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                var method = @class.FindMethod(s => s.HasMetadata("handle"))!;
                var serviceInvocation = new CSharpInvocationStatement($"_mediator.Send");

                if (endpointTemplate.Model.Parameters.Any())
                {
                    serviceInvocation.AddArgument($"req");
                }
                else
                {
                    var instantiation = new CSharpInvocationStatement($"new {endpointTemplate.GetTypeName(endpointTemplate.Model.InternalElement)}");
                    instantiation.WithoutSemicolon();
                    serviceInvocation.AddArgument(instantiation);
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

                invocation.AddMetadata("mediatr-dispatch", true);

                method.AddStatement(invocation);

                var returnStatement = endpointTemplate.GetReturnStatement();
                if (returnStatement is not null)
                {
                    method.AddStatement(returnStatement);
                }
            }, 2);
        }
    }
}