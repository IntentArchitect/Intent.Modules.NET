using Intent.Engine;
using Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerConfiguration;
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

namespace Intent.Modules.Aws.SecretsManager.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProgramExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.SecretsManager.ProgramExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var programTemplate = application.FindTemplateInstance<IProgramTemplate>(TemplateDependency.OnTemplate("App.Program"));
            if (programTemplate == null)
            {
                return;
            }

            var configTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(AwsSecretsManagerConfigurationTemplate.TemplateId));
            if (configTemplate == null)
            {
                return;
            }

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing(configTemplate.Namespace);
                file.AddUsing("Microsoft.Extensions.Configuration");

                if (programTemplate.ProgramFile.UsesMinimalHostingModel)
                {
                    programTemplate.ProgramFile.AddHostBuilderConfigurationStatement(new CSharpIfStatement("builder.Configuration.GetValue<bool?>(\"SecretsManager:Enabled\") == true"), @if =>
                    {
                        @if.AddStatement("builder.Configuration.ConfigureAwsSecretsManager(builder.Configuration);");
                    });
                }
                else
                {
                    programTemplate.ProgramFile.ConfigureAppConfiguration(true, (statements, parameters) =>
                    {
                        var builder = statements.FindStatement(s => s.HasMetadata("configuration-builder"));
                        if (builder is null)
                        {
                            builder = new CSharpStatement($"var configuration = {parameters[^1]}.Build();")
                                .AddMetadata("configuration-builder", true)
                                .SeparatedFromNext();
                            statements.InsertStatement(0, builder);
                        }
                        statements
                            .AddIfStatement(@"configuration.GetValue<bool?>(""SecretsManager:Enabled"") == true", @if =>
                            {
                                @if.AddStatement($"{parameters[^1]}.ConfigureAwsSecretsManager(configuration);");
                            });
                    });
                }
            }, 30);
        }
    }
}