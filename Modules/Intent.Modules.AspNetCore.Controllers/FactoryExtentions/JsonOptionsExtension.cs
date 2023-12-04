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

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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

            template.CSharpFile.AfterBuild(file =>
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

                    controllersStatement.WithoutSemicolon();
                    controllersStatement.InsertBelow(new CSharpInvocationStatement(".AddJsonOptions"), stmt =>
                    {
                        var invocation = (CSharpInvocationStatement)stmt;
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
            }, 11);
        }
    }
}