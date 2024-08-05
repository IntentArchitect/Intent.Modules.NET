using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.KeyVault.Templates.AzureKeyVaultConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureKeyVaultConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.KeyVault.AzureKeyVaultConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureKeyVaultConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureExtensionsAspNetCoreConfigurationSecrets(OutputTarget));
            AddNugetDependency(NugetPackages.AzureIdentity(OutputTarget));
            AddNugetDependency(NugetPackages.AzureSecurityKeyVaultSecrets(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Azure.Core")
                .AddUsing("Azure.Identity")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"AzureKeyVaultConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "ConfigureAzureKeyVault", method =>
                    {
                        method.Static();
                        method.AddParameter("IConfigurationBuilder", "builder", parm => parm.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement($@"var credential = GetTokenCredential(configuration);");
                        method.AddIfStatement(@"string.IsNullOrWhiteSpace(configuration[""KeyVault:Endpoint""])", block => block
                            .AddStatement(@"throw new InvalidOperationException(""Configuration 'KeyVault:Endpoint' is not set"");"));
                        method.AddStatement(@"builder.AddAzureKeyVault(new Uri(configuration[""KeyVault:Endpoint""]), credential);");
                    });
                    @class.AddMethod("TokenCredential", "GetTokenCredential", method =>
                    {
                        method.Static();
                        method.Private();
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddIfStatement(@"
!string.IsNullOrWhiteSpace(configuration[""KeyVault:TenantId""]) &&
!string.IsNullOrWhiteSpace(configuration[""KeyVault:ClientId""]) &&
!string.IsNullOrWhiteSpace(configuration[""KeyVault:Secret""])", stmt => stmt
                            .AddStatements(@"
// Manually specify the connection details for Azure Key Vault.
// Its recommended to store the 'Secret' inside the .NET User Secret's secrets.json file.
return new ClientSecretCredential(configuration[""KeyVault:TenantId""], configuration[""KeyVault:ClientId""], configuration[""KeyVault:Secret""]);"));

                        method.AddIfStatement(@"!string.IsNullOrWhiteSpace(configuration[""KeyVault:ClientId""])", stmt => stmt
                            .AddStatement("// Connect to Azure Key Vault using the configured App Client Id.")
                            .AddInvocationStatement("return new DefaultAzureCredential", stmt => stmt
                                .AddArgument(new CSharpObjectInitializerBlock("new DefaultAzureCredentialOptions")
                                    .AddInitStatement("ManagedIdentityClientId", @"configuration[""KeyVault:ClientId""]"))));

                        method.AddStatements(@"
// Use the default discovery mechanisms to connect to Azure Key Vault.
return new DefaultAzureCredential();");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("KeyVault", new
            {
                Enabled = false,
                Endpoint = "https://VAULT-NAME-HERE.vault.azure.net/",
                ClientId = "",
                Secret = "",
                TenantId = ""
            });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}