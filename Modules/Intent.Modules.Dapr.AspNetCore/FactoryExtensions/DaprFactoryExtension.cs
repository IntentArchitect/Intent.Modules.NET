using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DaprFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.DaprFactoryExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Publish(LaunchProfileHttpPortRequired.EventId, new Dictionary<string, string>());
            application.EventDispatcher.Publish(new AppSettingRegistrationRequest("DaprSidekick", new
            {
                Sidecar = new
                {
                    AppId = application.GetDaprApplicationName(application.Id),
                    ComponentsDirectory = "../dapr/components",
                    ConfigFile = "../dapr/config.yaml"
                }
            }));

            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate == null)
            {
                return;
            }

            startupTemplate.AddNugetDependency(NugetPackages.ManDaprSidekickAspNetCore(startupTemplate.OutputTarget));
            startupTemplate.CSharpFile.OnBuild(file =>
            {
                startupTemplate.StartupFile.ConfigureServices((statements, context) =>
                {
                    statements.Statements.Add(new CSharpInvocationStatement($"{context.Services}.AddDaprSidekick").AddArgument(context.Configuration));

                    // Until we can make the "AddController" statement in the Intent.AspNetCore.Controllers be
                    // a CSharpInvocationStatement that supports method chaining, this will have to do.
                    // It's our original hack approach anyway and turning this into a CSharpMethodChainStatement will
                    // only make the CSharpInvocationStatement change later difficult. 
                    file.AfterBuild(nestedFile =>
                    {
                        var statementsToCheck = new List<CSharpStatement>();
                        ExtractPossibleStatements(statements, statementsToCheck);

                        var lastConfigStatement = (CSharpInvocationStatement)statementsToCheck.Last(p => p.HasMetadata("configure-services-controllers"));
                        var addDaprStatement = statements.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "dapr")
                            as CSharpInvocationStatement;
                        if (addDaprStatement is null)
                        {
                            addDaprStatement = new CSharpInvocationStatement(".AddDapr");
                            addDaprStatement.AddMetadata("configure-services-controllers", "dapr");
                            lastConfigStatement.InsertBelow(addDaprStatement);
                        }

                        lastConfigStatement.WithoutSemicolon();
                    });
                });
                startupTemplate.StartupFile.ConfigureApp((statements, _) =>
                {
                    statements
                        .FindStatement(x => x.ToString()!.Contains(".UseHttpsRedirection()"))?
                        .Remove();
                });
            }, 15);
        }

        private static void ExtractPossibleStatements(IHasCSharpStatements targetBlock, List<CSharpStatement> statementsToCheck)
        {
            foreach (var statement in targetBlock.Statements)
            {
                if (statement is CSharpInvocationStatement)
                {
                    statementsToCheck.Add(statement);
                }
                else if (statement is IHasCSharpStatements container)
                {
                    foreach (var nested in container.Statements)
                    {
                        statementsToCheck.Add(nested);
                    }
                }
            }
        }
    }
}