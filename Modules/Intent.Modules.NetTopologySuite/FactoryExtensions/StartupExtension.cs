using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.NetTopologySuite.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.NetTopologySuite.StartupExtension";

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
            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate is null)
            {
                return;
            }

            startupTemplate.AddNugetDependency(NugetPackages.NetTopologySuiteIoGeoJson4Stj);
            startupTemplate.CSharpFile.OnBuild(file =>
            {
                startupTemplate.StartupFile.ConfigureServices((statements, context) =>
                {
                    // Until we can make the "AddController" statement in the Intent.AspNetCore.Controllers be
                    // a CSharpInvocationStatement that supports method chaining, this will have to do.
                    // It's our original hack approach anyway and turning this into a CSharpMethodChainStatement will
                    // only make the CSharpInvocationStatement change later difficult. 
                    file.AfterBuild(nestedFile =>
                    {
                        var statementsToCheck = new List<CSharpStatement>();
                        ExtractPossibleStatements(statements, statementsToCheck);

                        var lastConfigStatement = (CSharpInvocationStatement)statementsToCheck.Last(p => p.HasMetadata("configure-services-controllers"));
                        var addJsonOptionsStatement = statements.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "json")
                            as CSharpInvocationStatement;
                        if (addJsonOptionsStatement is null)
                        {
                            addJsonOptionsStatement = new CSharpInvocationStatement(".AddJsonOptions");
                            addJsonOptionsStatement.AddMetadata("configure-services-controllers", "json");
                            lastConfigStatement.InsertBelow(addJsonOptionsStatement);
                        }

                        lastConfigStatement.WithoutSemicolon();

                        var lambda = addJsonOptionsStatement.Statements.FirstOrDefault() as CSharpLambdaBlock;
                        if (lambda is null)
                        {
                            lambda = new CSharpLambdaBlock("options");
                            addJsonOptionsStatement.AddArgument(lambda);
                        }

                        lambda.AddStatement($@"options.JsonSerializerOptions.Converters.Add(new {startupTemplate.UseType("NetTopologySuite.IO.Converters.GeoJsonConverterFactory")}());");
                    });
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