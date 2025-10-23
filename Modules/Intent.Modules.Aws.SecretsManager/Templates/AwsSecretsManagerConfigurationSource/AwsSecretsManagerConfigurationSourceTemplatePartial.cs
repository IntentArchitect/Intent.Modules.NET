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

namespace Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerConfigurationSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AwsSecretsManagerConfigurationSourceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.SecretsManager.AwsSecretsManagerConfigurationSource";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AwsSecretsManagerConfigurationSourceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AwsSecretsManagerConfigurationProvider", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.Extensions.Configuration.ConfigurationProvider"));

                    @class.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "region")
                            .AddParameter("string", "secretName");
                    });

                    @class.AddField("string", "SecretVersionStage", @field =>
                    {
                        field.PrivateConstant("\"AWSCURRENT\"");
                    });

                    @class.AddMethod("void", "Load", loadMethod =>
                    {
                        loadMethod.Override();
                        loadMethod.AddObjectInitStatement("var secretJson", "GetSecret();");

                        AddUsing("System.Text");
                        loadMethod.AddUsingBlock($"var ms = new {UseType("System.IO.MemoryStream")}(Encoding.UTF8.GetBytes(secretJson))", @using =>
                        {
                            var tempConfigInvoc = new CSharpInvocationStatement("new ConfigurationBuilder")
                            .AddInvocation("AddJsonStream", invoc => invoc.OnNewLine().AddArgument("ms"))
                            .AddInvocation("Build", invoc => invoc.OnNewLine());

                            @using.AddObjectInitStatement("var tempConfig", tempConfigInvoc);

                            AddUsing("System.Linq");
                            var toDictionaryInvoc = new CSharpInvocationStatement("tempConfig.AsEnumerable").OnNewLine().AddArgument("makePathsRelative: false")
                                .AddInvocation("Where", invoc => invoc.OnNewLine().AddArgument("kv => kv.Value is not null"))
                                .AddInvocation("ToDictionary", invoc => invoc.OnNewLine().AddArgument("kv => kv.Key, kv => kv.Value").AddArgument("StringComparer.OrdinalIgnoreCase"));

                            @using.AddObjectInitStatement("Data", toDictionaryInvoc);
                        });
                    });

                    @class.AddMethod("string", "GetSecret", getMethod =>
                    {
                        getMethod.Private();

                        var requestBlock = new CSharpObjectInitializerBlock($"new {UseType("Amazon.SecretsManager.Model.GetSecretValueRequest")}")
                            .AddInitStatement("SecretId", "secretName")
                            .AddInitStatement("VersionStage", "SecretVersionStage").WithSemicolon();
                        getMethod.AddObjectInitStatement("var request", requestBlock);

                        AddUsing("Amazon");
                        var clientInstance = new CSharpInvocationStatement($"new {UseType("Amazon.SecretsManager.AmazonSecretsManagerClient")}")
                            .AddArgument(new CSharpInvocationStatement("RegionEndpoint.GetBySystemName")
                                    .AddArgument("region")
                                    .WithoutSemicolon()
                            ).WithoutSemicolon();

                        var clientInit = new CSharpObjectInitStatement("var client", clientInstance);
                        getMethod.AddUsingBlock(clientInit.ToString(), @using =>
                        {
                            var getSecretInvoc = new CSharpInvocationStatement("client.GetSecretValueAsync")
                                .AddArgument("request")
                                .AddInvocation("GetAwaiter")
                                .AddInvocation("GetResult");
                            @using.AddObjectInitStatement("var response", getSecretInvoc);

                            @using.AddIfStatement("!string.IsNullOrEmpty(response.SecretString)", @if =>
                            {
                                @if.AddReturn("response.SecretString");
                            });

                            @using.AddUsingBlock("var reader = new StreamReader(response.SecretBinary)", readerUsing =>
                            {
                                readerUsing.AddObjectInitStatement("var base64", "reader.ReadToEnd();");
                                readerUsing.AddObjectInitStatement("var bytes", "Convert.FromBase64String(base64);");

                                readerUsing.AddReturn("Encoding.UTF8.GetString(bytes)");
                            });
                        });

                    });
                }).AddClass("AwsSecretsManagerConfigurationSource", @class =>
                {
                    @class.ImplementsInterface(UseType("Microsoft.Extensions.Configuration.IConfigurationSource"));

                    @class.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "region")
                            .AddParameter("string", "secretName");
                    });

                    @class.AddMethod(UseType("Microsoft.Extensions.Configuration.IConfigurationProvider"), "Build", method =>
                    {
                        method.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfigurationBuilder"), "builder");
                        var returnInvoc = new CSharpInvocationStatement("new AwsSecretsManagerConfigurationProvider")
                            .AddArgument("region")
                            .AddArgument("secretName")
                            .WithoutSemicolon();

                        method.AddReturn(returnInvoc);
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            var config = CSharpFile.GetConfig();
            config.FileName = "AwsSecretsManagerConfigurationSource";
            return config;
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}