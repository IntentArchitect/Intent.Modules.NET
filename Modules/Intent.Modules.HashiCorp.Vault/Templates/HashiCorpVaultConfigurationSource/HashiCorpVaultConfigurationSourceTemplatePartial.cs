using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultOptions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultConfigurationSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HashiCorpVaultConfigurationSourceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HashiCorp.Vault.HashiCorpVaultConfigurationSource";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HashiCorpVaultConfigurationSourceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath())
                .AddClass("HashiCorpVaultConfigurationSource", @class =>
                {
                    @class.ImplementsInterface(UseType("Microsoft.Extensions.Configuration.IConfigurationSource"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("HashiCorpVaultEntry", "hashiCorpVaultEntry", @param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod(UseType("Microsoft.Extensions.Configuration.IConfigurationProvider"), "Build", method =>
                    {
                        method.AddParameter("IConfigurationBuilder", "builder");
                        method.WithExpressionBody("new HashiCorpVaultConfigurationProvider(_hashiCorpVaultEntry)");
                    });
                })
                .AddClass("HashiCorpVaultConfigurationProvider", @class =>
                {
                    @class.ImplementsInterface("ConfigurationProvider");
                    @class.AddField(UseType("VaultSharp.IVaultClient"), "_vaultClient", param => param.PrivateReadOnly());
                    @class.AddField(UseType("System.Timers.Timer?"), "_reloadTimer", param => param.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("HashiCorpVaultEntry", "hashiCorpVaultEntry", param =>
                        {
                            param.IntroduceReadonlyField();
                        });

                        ctor.AddObjectInitStatement("var auth", "GetAuthMethod(hashiCorpVaultEntry.AuthMethod);");
                        ctor.AddObjectInitStatement("_vaultClient", "new VaultClient(new VaultClientSettings(hashiCorpVaultEntry.Url.ToString(), auth));");

                        ctor.AddIfStatement("hashiCorpVaultEntry.CacheTimeoutInSeconds != default", @if =>
                        {
                            var init = new CSharpStatement(@"new  System.Timers.Timer
            {
                Enabled = true,
                Interval = TimeSpan.FromSeconds(hashiCorpVaultEntry.CacheTimeoutInSeconds).TotalMilliseconds,
                AutoReset = true
            };");

                            @if.AddObjectInitStatement("_reloadTimer", init);

                            @if.AddStatement("_reloadTimer.Elapsed += (_, _) => Load();");
                        });
                    });

                    @class.AddMethod(UseType("VaultSharp.V1.AuthMethods.IAuthMethodInfo"), "GetAuthMethod", method =>
                    {
                        method.Static();
                        method.Private();
                        method.AddParameter("HashiCorpVaultAuthMethod", "authMethod");

                        method.AddIfStatement("authMethod.AppRole is not null", @if =>
                        {
                            @if.AddReturn($"new {UseType("VaultSharp.V1.AuthMethods.AppRole.AppRoleAuthMethodInfo")}(authMethod.AppRole.RoleId, authMethod.AppRole.SecretId)");
                        });

                        method.AddIfStatement("authMethod.UserPass is not null", @if =>
                        {
                            @if.AddReturn($"new {UseType("VaultSharp.V1.AuthMethods.UserPass.UserPassAuthMethodInfo")}(authMethod.UserPass.Username, authMethod.UserPass.Password)");
                        });

                        method.AddIfStatement("authMethod.Token is not null", @if =>
                        {
                            @if.AddReturn($"new {UseType("VaultSharp.V1.AuthMethods.Token.TokenAuthMethodInfo")}(authMethod.Token.Token)");
                        });

                        method.AddStatement("throw new InvalidOperationException(\"No Auth Method was specified\");");

                    });

                    @class.AddMethod("void", "Load", method =>
                    {
                        method.Override();

                        method.AddObjectInitStatement("var keys", "_vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: _hashiCorpVaultEntry.Path, mountPoint: _hashiCorpVaultEntry.MountPoint).GetAwaiter().GetResult();");
                        method.AddInvocationStatement("Data.Clear");
                        method.AddStatement("");

                        method.AddObjectInitStatement("var flattenedKeys", "keys.Data.Data.SelectMany(JsonConfigurationFileParser.Convert);");
                        method.AddForEachStatement("entry", "flattenedKeys", @foreach =>
                        {
                            @foreach.AddStatement("Data.Add(entry.Key, entry.Value);");
                        });
                    });

                    @class.AddNestedClass("JsonConfigurationFileParser", jsonClass =>
                    {
                        jsonClass.Internal();

                        jsonClass.AddConstructor(ctor =>
                        {
                            ctor.Private();
                        });

                        jsonClass.AddField("Dictionary<string, string?>", "_data", @field =>
                        {
                            field.PrivateReadOnly();
                            field.WithAssignment(new CSharpStatement("new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)"));
                        });

                        jsonClass.AddField("Stack<string>", "_paths", @field =>
                        {
                            field.PrivateReadOnly();
                            field.WithAssignment(new CSharpStatement("new Stack<string>()"));
                        });

                        jsonClass.AddMethod("IDictionary<string, string?>", "Convert", method =>
                        {
                            method.Static();
                            method.AddParameter("KeyValuePair<string, object>", "pair");
                            method.WithExpressionBody("new JsonConfigurationFileParser().ConvertObject(pair)");
                        });

                        jsonClass.AddMethod("Dictionary<string, string?>", "ConvertObject", method =>
                        {
                            method.Private();
                            method.AddParameter("KeyValuePair<string, object>", "pair");

                            method.AddIfStatement($"pair.Value is not {UseType("System.Text.Json.JsonElement")} jsonElement", @if =>
                            {
                                @if.AddReturn("_data");
                            });

                            method.AddSwitchStatement("jsonElement.ValueKind", @switch =>
                            {
                                @switch.AddCase("JsonValueKind.Object", @case =>
                                {
                                    @case.AddInvocationStatement("VisitObjectElement", invoc =>
                                    {
                                        invoc.AddArgument("jsonElement");
                                        invoc.AddArgument("pair.Key");
                                    });
                                    @case.WithBreak();
                                });

                                @switch.AddCase("JsonValueKind.Array", @case =>
                                {
                                    @case.AddInvocationStatement("VisitArrayElement", invoc =>
                                    {
                                        invoc.AddArgument("jsonElement");
                                        invoc.AddArgument("pair.Key");
                                    });
                                    @case.WithBreak();
                                });
                                @switch.AddCase("JsonValueKind.String");
                                @switch.AddCase("JsonValueKind.Number");
                                @switch.AddCase("JsonValueKind.True");
                                @switch.AddCase("JsonValueKind.False", @case =>
                                {
                                    @case.AddInvocationStatement("_data.Add", invoc =>
                                    {
                                        invoc.AddArgument("pair.Key");
                                        invoc.AddArgument("pair.Value.ToString()");
                                    });
                                    @case.WithBreak();
                                });
                                @switch.AddCase("JsonValueKind.Null", @case =>
                                {
                                    @case.AddInvocationStatement("_data.Add", invoc =>
                                    {
                                        invoc.AddArgument("pair.Key");
                                        invoc.AddArgument("null");
                                    });
                                    @case.WithBreak();
                                });
                                @switch.AddCase("JsonValueKind.Undefined");
                                @switch.AddDefault(@case =>
                                {
                                    @case.AddStatement("throw new FormatException($\"Unsupported JSON Token: {jsonElement.ValueKind}\");");
                                });
                            });

                            method.AddReturn("_data");
                        });

                        jsonClass.AddMethod("void", "VisitObjectElement", method =>
                        {
                            method.Private();
                            method.AddParameter("JsonElement", "element");
                            method.AddParameter("string", "mainKey");

                            method.AddObjectInitStatement("var isEmpty", "true;");

                            method.AddForEachStatement("property", "element.EnumerateObject()", @foreach =>
                            {
                                @foreach.AddObjectInitStatement("isEmpty", "false;");
                                @foreach.AddInvocationStatement("EnterContext", cfg =>
                                {
                                    cfg.AddArgument("property.Name");
                                });
                                @foreach.AddInvocationStatement("VisitValue", cfg =>
                                {
                                    cfg.AddArgument("property.Value").AddArgument("mainKey");
                                });
                                @foreach.AddInvocationStatement("ExitContext");
                            });

                            method.AddStatement("");
                            method.AddInvocationStatement("SetNullIfElementIsEmpty", cfg => cfg.AddArgument("isEmpty"));
                        });

                        jsonClass.AddMethod("void", "VisitArrayElement", method =>
                        {
                            method.Private();
                            method.AddParameter("JsonElement", "element");
                            method.AddParameter("string", "mainKey");

                            method.AddObjectInitStatement("int index", "0;");

                            method.AddForEachStatement("arrayElement", "element.EnumerateArray()", @foreach =>
                            {
                                @foreach.AddInvocationStatement("EnterContext", cfg =>
                                {
                                    cfg.AddArgument("index.ToString()");
                                });
                                @foreach.AddInvocationStatement("VisitValue", cfg =>
                                {
                                    cfg.AddArgument("arrayElement").AddArgument("mainKey");
                                });
                                @foreach.AddInvocationStatement("ExitContext");
                                @foreach.AddStatement("index++;");
                            });

                            method.AddStatement("");
                            method.AddInvocationStatement("SetNullIfElementIsEmpty", cfg => cfg.AddArgument("isEmpty: index == 0"));
                        });

                        jsonClass.AddMethod("void", "SetNullIfElementIsEmpty", method =>
                        {
                            method.Private();
                            method.AddParameter("bool", "isEmpty");

                            method.AddIfStatement("isEmpty && _paths.Count > 0", @if =>
                            {
                                @if.AddObjectInitStatement("_data[_paths.Peek()]", "null;");
                            });
                        });

                        jsonClass.AddMethod("void", "VisitValue", method =>
                        {
                            method.Private();
                            method.AddParameter("JsonElement", "value");
                            method.AddParameter("string", "mainKey");

                            AddUsing("System.Diagnostics");
                            method.AddStatement("Debug.Assert(_paths.Count > 0);");

                            method.AddSwitchStatement("value.ValueKind", @switch =>
                            {
                                @switch.AddCase("JsonValueKind.Object", @case =>
                                {
                                    @case.AddInvocationStatement("VisitObjectElement", invoc =>
                                    {
                                        invoc.AddArgument("value");
                                        invoc.AddArgument("mainKey");
                                    });
                                    @case.WithBreak();
                                });

                                @switch.AddCase("JsonValueKind.Array", @case =>
                                {
                                    @case.AddInvocationStatement("VisitArrayElement", invoc =>
                                    {
                                        invoc.AddArgument("value");
                                        invoc.AddArgument("mainKey");
                                    });
                                    @case.WithBreak();
                                });
                                @switch.AddCase("JsonValueKind.Number");
                                @switch.AddCase("JsonValueKind.String");
                                @switch.AddCase("JsonValueKind.True");
                                @switch.AddCase("JsonValueKind.False");
                                @switch.AddCase("JsonValueKind.Null", @case =>
                                {
                                    @case.AddObjectInitStatement("string key", "$\"{mainKey}:{_paths.Peek()}\".Replace(\"__\", \":\");");
                                    @case.AddIfStatement("_data.ContainsKey(key)", @if =>
                                    {
                                        @if.AddStatement("throw new FormatException($\"Key is duplicated: {mainKey}\");");
                                    });
                                    @case.AddStatement("");
                                    @case.AddObjectInitStatement("_data[key] ", "value.ToString();");
                                    @case.WithBreak();
                                });
                                @switch.AddCase("JsonValueKind.Undefined");
                                @switch.AddDefault(@case =>
                                {
                                    @case.AddStatement("throw new FormatException($\"Unsupported JSON Token: {value.ValueKind}\");");
                                });
                            });
                        });

                        jsonClass.AddMethod("void", "EnterContext", method =>
                        {
                            method.Private();
                            method.AddParameter("string", "context");
                            method.WithExpressionBody("_paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context)");
                        });

                        jsonClass.AddMethod("void", "ExitContext", method =>
                        {
                            method.Private();
                            method.WithExpressionBody("_paths.Pop()");
                        });
                    });

                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{ClassNameBase}Source",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public string ClassNameBase => "HashiCorpVaultConfiguration";

        public string Nullable => OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;
    }
}