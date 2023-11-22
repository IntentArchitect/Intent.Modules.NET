using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.ODataQuery.Settings;
using Intent.Modules.Common;
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
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Distribution.WebApi.Startup));

            template?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();

                if (@class.FindMethod("ConfigureServices")
                        .FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not
                    CSharpInvocationStatement controllersStatement)
                {
                    return;
                }

                template.AddUsing("Microsoft.AspNetCore.OData");
                controllersStatement.WithoutSemicolon();
                controllersStatement.InsertBelow(new CSharpInvocationStatement(".AddOData"), stmt =>
                {
                    CSharpInvocationStatement invocation = (CSharpInvocationStatement)stmt;
                    CSharpLambdaBlock lambda;
                    if (!invocation.Statements.Any())
                    {
                        lambda = new CSharpLambdaBlock("options");
                        invocation.AddArgument(lambda);
                    }
                    else
                    {
                        lambda = invocation.Statements.First() as CSharpLambdaBlock;
                    }
                    var odataConfig = new StringBuilder();
                    var settings = template.ExecutionContext.Settings.GetODataQuerySettings();
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
            }, 10);
        }
    }
}