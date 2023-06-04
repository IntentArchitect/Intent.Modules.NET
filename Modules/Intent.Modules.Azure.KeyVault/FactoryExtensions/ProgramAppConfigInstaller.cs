using System.Linq;
using Intent.Engine;
using Intent.Modules.Azure.KeyVault.Templates.AzureKeyVaultConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Azure.KeyVault.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class ProgramAppConfigInstaller : FactoryExtensionBase
{
    public override string Id => "Intent.Azure.KeyVault.ProgramAppConfigInstaller";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var programTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("App.Program"));
        if (programTemplate == null)
        {
            return;
        }

        var configTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(AzureKeyVaultConfigurationTemplate.TemplateId));
        if (configTemplate == null)
        {
            return;
        }

        programTemplate.CSharpFile.OnBuild(file =>
        {
            file.AddUsing(configTemplate.Namespace);
            file.AddUsing("Microsoft.Extensions.Configuration");
        
            var @class = file.Classes.First();
            var hostBuilder = @class.FindMethod("CreateHostBuilder");
            var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
            hostBuilderChain.Statements.Last().InsertAbove(new CSharpInvocationStatement("ConfigureAppConfiguration")
                .WithoutSemicolon()
                .AddArgument(new CSharpLambdaBlock("(context, config)")
                    .AddStatement("var configuration = config.Build();")
                    .AddIfStatement(@"configuration.GetValue<bool?>(""KeyVault:Enabled"") == true", block => block
                        .AddStatement(@"config.ConfigureAzureKeyVault(configuration);")))); 
        }, 30);
    }
}