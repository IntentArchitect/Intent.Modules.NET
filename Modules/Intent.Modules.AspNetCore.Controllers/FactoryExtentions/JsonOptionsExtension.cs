using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Settings;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class JsonOptionsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.JsonOptionsExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            SetupEnumsAsStrings(application);
        }

        private static void SetupEnumsAsStrings(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            if (template is null)
            {
                return;
            }

            var enumsAsStrings = template.ExecutionContext.Settings.GetAPISettings().SerializeEnumsAsStrings();
            var ignoreCycles = template.ExecutionContext.Settings.GetAPISettings().OnSerializationIgnoreJSONReferenceCycles();
            if (!enumsAsStrings && !ignoreCycles)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var startup = template.StartupFile;
                startup.ConfigureServices((statements, context) =>
                {
                    if (statements.FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not
                        CSharpInvocationStatement controllersStatement)
                    {
                        return;
                    }

                    file.AddUsing("System.Text.Json.Serialization");

                    // Until we can make the "AddController" statement in the Intent.AspNetCore.Controllers be
                    // a CSharpInvocationStatement that supports method chaining, this will have to do.
                    // It's our original hack approach anyway and turning this into a CSharpMethodChainStatement will
                    // only make the CSharpInvocationStatement change later difficult. 
                    template.CSharpFile.AfterBuild(nestedFile =>
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

                        if (enumsAsStrings)
                        {
                            lambda.AddStatement(
                                "options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());");
                        }

                        if (ignoreCycles)
                        {
                            lambda.AddStatement(
                                "options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;");
                        }
                    });
                });
            }, 14);
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