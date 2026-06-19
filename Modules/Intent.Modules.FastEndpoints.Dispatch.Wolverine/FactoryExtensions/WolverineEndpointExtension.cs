using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.FastEndpoints.Dispatch.Wolverine.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Dispatch.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineEndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.FastEndpoints.Dispatch.Wolverine.WolverineEndpointExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var endpointTemplates = application.FindTemplateInstances<EndpointTemplate>(EndpointTemplate.TemplateId);
            foreach (var endpointTemplate in endpointTemplates)
            {
                if (endpointTemplate.Model is not WolverineEndpointModel)
                {
                    continue;
                }
                InstallWolverineDispatch(endpointTemplate);
            }
        }

        private void InstallWolverineDispatch(EndpointTemplate endpointTemplate)
        {
            endpointTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(endpointTemplate.UseType("Wolverine.IMessageBus"), "sender",
                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                var method = @class.FindMethod(s => s.HasMetadata("handle"))!;

                CSharpStatement invocation;
                if (endpointTemplate.Model!.ReturnType is not null)
                {
                    var returnTypeName = endpointTemplate.GetTypeName(endpointTemplate.Model.ReturnType);
                    var defaultValue = GetDefaultValue(returnTypeName);
                    method.AddStatement($"var result = {defaultValue};");

                    if (endpointTemplate.Model.Parameters.Any())
                    {
                        invocation = new CSharpAssignmentStatement("result", new CSharpAwaitExpression(new CSharpInvocationStatement($"_sender.InvokeAsync<{returnTypeName}>").AddArgument("req").AddArgument("ct")));
                    }
                    else
                    {
                        var instantiation = new CSharpInvocationStatement($"new {endpointTemplate.GetTypeName(endpointTemplate.Model.InternalElement)}");
                        instantiation.WithoutSemicolon();
                        invocation = new CSharpAssignmentStatement("result", new CSharpAwaitExpression(new CSharpInvocationStatement($"_sender.InvokeAsync<{returnTypeName}>").AddArgument(instantiation).AddArgument("ct")));
                    }
                }
                else
                {
                    if (endpointTemplate.Model.Parameters.Any())
                    {
                        invocation = new CSharpAwaitExpression(new CSharpInvocationStatement("_sender.InvokeAsync").AddArgument("req").AddArgument("ct"));
                    }
                    else
                    {
                        var instantiation = new CSharpInvocationStatement($"new {endpointTemplate.GetTypeName(endpointTemplate.Model.InternalElement)}");
                        instantiation.WithoutSemicolon();
                        invocation = new CSharpAwaitExpression(new CSharpInvocationStatement("_sender.InvokeAsync").AddArgument(instantiation).AddArgument("ct"));
                    }
                }

                invocation.AddMetadata("wolverine-dispatch", true);
                method.AddStatement(invocation);

                var returnStatement = endpointTemplate.GetReturnStatement();
                if (returnStatement is not null)
                {
                    method.AddStatement(returnStatement);
                }
            }, 2);
        }

        private static string GetDefaultValue(string type) => type switch
        {
            "Guid" => "Guid.Empty",
            _ => $"default({type})"
        };
    }
}
