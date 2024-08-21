using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HashiCorpVaultConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HashiCorp.Vault.HashiCorpVaultConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HashiCorpVaultConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.VaultSharp(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("System.Linq")
                .AddClass($"HashiCorpVaultConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "ConfigureHashiCorpVault", method =>
                    {
                        method.Static();
                        method.AddParameter("IConfigurationBuilder", "builder", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement($"var options = new {this.GetHashiCorpVaultOptionsName()}();");
                        method.AddStatement(@"configuration.GetSection(""HashiCorpVault"").Bind(options);");
                        method.AddForEachStatement("vault", "options.Vaults", loop => loop
                            .AddStatement(@"var shorthandConfig = configuration.GetChildren().Where(p => p.Key.StartsWith($""{vault.Name}_"")).ToArray();")
                            .AddStatement("vault.ApplyShorthandConfig(shorthandConfig);")
                            .AddStatement($"builder.Add(new {this.GetHashiCorpVaultConfigurationSourceName()}(vault));"));
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("HashiCorpVault", new
            {
                Enabled = true,
                Vaults = new[]
                {
                    new
                    {
                        Name = "Developer Vault",
                        Url = new Uri("http://127.0.0.1:8200"),
                        AuthMethod = new
                        {
                            Token = new
                            {
                                Token = "root_token"
                            }
                        },
                        Path = "creds",
                        MountPoint = "secret",
                        CacheTimeoutInSeconds = 30
                    }
                }
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