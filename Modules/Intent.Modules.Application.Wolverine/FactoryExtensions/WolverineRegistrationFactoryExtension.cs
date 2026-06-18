using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineRegistrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Wolverine.WolverineRegistrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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
            var programTemplate = application.FindTemplateInstance<IProgramTemplate>("App.Program");
            var infraDiTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);

            if (programTemplate != null)
            {
                programTemplate.AddNugetDependency(NugetPackages.WolverineFx(programTemplate.OutputTarget));

                programTemplate.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("Wolverine");

                    var wolverineConfigType = programTemplate.GetTypeName("Intent.Application.Wolverine.WolverineConfiguration");

                    programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" },
                        (lambdaBlock, parameters) =>
                        {
                            var opts = parameters[0];
                            lambdaBlock.Statements.Clear();
                            lambdaBlock.AddStatement($"{wolverineConfigType}.Configure({opts});");
                        });
                });
            }

            if (infraDiTemplate != null)
            {
                infraDiTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("AddInfrastructure");
                    if (method != null)
                    {
                        var authMiddleware = infraDiTemplate.GetTypeName("Intent.Application.Wolverine.AuthorizationMiddleware");
                        var valMiddleware = infraDiTemplate.GetTypeName("Intent.Application.Wolverine.ValidationMiddleware");
                        var logMiddleware = infraDiTemplate.GetTypeName("Intent.Application.Wolverine.LoggingMiddleware");
                        var perfMiddleware = infraDiTemplate.GetTypeName("Intent.Application.Wolverine.PerformanceMiddleware");
                        var errMiddleware = infraDiTemplate.GetTypeName("Intent.Application.Wolverine.UnhandledExceptionMiddleware");
                        var uowMiddleware = infraDiTemplate.GetTypeName("Intent.Application.Wolverine.UnitOfWorkMiddleware");

                        method.AddStatement($"services.AddTransient<{authMiddleware}>();");
                        method.AddStatement($"services.AddTransient<{valMiddleware}>();");
                        method.AddStatement($"services.AddTransient<{logMiddleware}>();");
                        method.AddStatement($"services.AddTransient<{perfMiddleware}>();");
                        method.AddStatement($"services.AddTransient<{errMiddleware}>();");
                        method.AddStatement($"services.AddTransient<{uowMiddleware}>();");
                    }
                });
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