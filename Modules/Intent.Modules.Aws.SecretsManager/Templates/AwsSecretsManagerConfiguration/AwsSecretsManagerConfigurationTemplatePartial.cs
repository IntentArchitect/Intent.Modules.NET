using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerOptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AwsSecretsManagerConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.SecretsManager.AwsSecretsManagerConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AwsSecretsManagerConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AWSSDKSecretsManager(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AwsSecretsManagerConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "ConfigureAwsSecretsManager", method =>
                    {
                        method.Static();
                        method.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfigurationBuilder"), "builder", p => p.WithThisModifier())
                            .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        AddTemplateDependency(AwsSecretsManagerOptionsTemplate.TemplateId);
                        method.AddObjectInitStatement("var options", "new AwsSecretsManagerOptions();");
                        method.AddInvocationStatement("configuration.GetSection", invoc =>
                        {
                            invoc.AddArgument("\"SecretsManager\"");
                            invoc.AddInvocation("Bind", bindInvoc =>
                            {
                                bindInvoc.AddArgument("options");
                            });
                        });

                        method.AddForEachStatement("secret", "options.Secrets", @for =>
                        {
                            @for.AddInvocationStatement("builder.Add", invoc =>
                            {
                                var sourceInvoc = new CSharpInvocationStatement("new AwsSecretsManagerConfigurationSource")
                                    .AddArgument("secret.Region")
                                    .AddArgument("secret.SecretName")
                                    .WithoutSemicolon();

                                //AddTemplateDependency(AwsSecretsManagerOptionsTemplate.TemplateId);
                                invoc.AddArgument(sourceInvoc);
                            });

                        });
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("SecretsManager", new
            {
                Enabled = true,
                Secrets = new[]
                {
                    new
                    {
                        Region = "us-east-1",
                        SecretName = $"{OutputTarget.ApplicationName().ToLower().Replace(" ", "")}/sample/secrets"
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