using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.ServiceCallHandlers.Templates;
using Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandlerImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.ServiceCallHandlers.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceImplementationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.ServiceCallHandlers.ServiceImplementationExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Application.Implementation");
            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("Microsoft.Extensions.DependencyInjection");

                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField());

                    foreach (var method in @class.Methods.Where(p => p.HasMetadata("model")))
                    {
                        var model = method.GetMetadata<OperationModel>("model");
                        var schTemplate = application.FindTemplateInstance<ServiceCallHandlerImplementationTemplate>("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", model);
                        if (schTemplate is null)
                        {
                            continue;
                        }

                        method.Attributes.OfType<CSharpIntentManagedAttribute>().First().WithBodyFully();

                        method.Statements.Clear();
                        method.AddStatement(
                            $"var sch = ({template.GetTypeName(ServiceCallHandlerImplementationTemplate.TemplateId, model)})_serviceProvider.GetRequiredService(typeof({template.GetTypeName(ServiceCallHandlerImplementationTemplate.TemplateId, model)}));");

                        var resultPart = (model.ReturnType is not null ? "var result = " : string.Empty);
                        var asyncPart = (model.IsAsync() ? "await " : string.Empty);

                        var handlerInvoke = new CSharpInvocationStatement(
                            $"{resultPart}{asyncPart}sch.Handle");
                        foreach (var parameter in method.Parameters)
                        {
                            handlerInvoke.AddArgument(parameter.Name);
                        }

                        method.AddStatement(handlerInvoke);
                        if (model.ReturnType is not null)
                        {
                            method.AddStatement("return result;");
                        }
                    }
                }, 1000);
            }
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // Your custom logic here.
        }
    }
}