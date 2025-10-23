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

namespace Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AwsSecretsManagerOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.SecretsManager.AwsSecretsManagerOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AwsSecretsManagerOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AwsSecretsManagerOptions", @class =>
                {
                    @class.AddProperty("bool", "Enabled")
                        .AddProperty(UseType("System.Collections.Generic.List<AwsSecretsManagerSecret>"), "Secrets", prop =>
                        {
                            prop.WithInitialValue("new List<AwsSecretsManagerSecret>()");
                        });
                        
                })
                .AddClass("AwsSecretsManagerSecret", @class =>
                {
                    @class.AddProperty("string", "Region")
                        .AddProperty("string", "SecretName");
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