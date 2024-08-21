using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.ODataQuery.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.ODataQuery.StartupExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate is null)
            {
                return;
            }

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
                        nestedFile.AddUsing("Microsoft.AspNetCore.OData");

                        var statementsToCheck = new List<CSharpStatement>();
                        ExtractPossibleStatements(statements, statementsToCheck);

                        var lastConfigStatement = (CSharpInvocationStatement)statementsToCheck.Last(p => p.HasMetadata("configure-services-controllers"));
                        var addODataStatement = statements.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "odata")
                            as CSharpInvocationStatement;
                        if (addODataStatement is null)
                        {
                            addODataStatement = new CSharpInvocationStatement(".AddOData");
                            addODataStatement.AddMetadata("configure-services-controllers", "odata");
                            lastConfigStatement.InsertBelow(addODataStatement);
                        }

                        lastConfigStatement.WithoutSemicolon();

                        var lambda = addODataStatement.Statements.FirstOrDefault() as CSharpLambdaBlock;
                        if (lambda is null)
                        {
                            lambda = new CSharpLambdaBlock("options");
                            addODataStatement.AddArgument(lambda);
                        }

                        var odataConfig = new StringBuilder();
                        var settings = startupTemplate.ExecutionContext.Settings.GetODataQuerySettings();
                        if (settings.AllowFilterOption())
                        {
                            odataConfig.Append(".Filter()");
                        }
                        if (settings.AllowOrderByOption())
                        {
                            odataConfig.Append(".OrderBy()");
                        }
                        if (settings.AllowExpandOption())
                        {
                            odataConfig.Append(".Expand()");
                        }
                        if (settings.AllowSelectOption())
                        {
                            odataConfig.Append(".Select()");
                        }
                        if (!string.IsNullOrEmpty(settings.MaxTop()))
                        {
                            if (int.TryParse(settings.MaxTop(), out var _))
                            {
                                odataConfig.Append($".SetMaxTop({settings.MaxTop()})");
                            }
                        }
                        lambda.AddStatement($"options{odataConfig};");
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