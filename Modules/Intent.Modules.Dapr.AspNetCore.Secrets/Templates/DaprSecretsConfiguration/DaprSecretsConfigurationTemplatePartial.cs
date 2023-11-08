using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Secrets.Templates.DaprSecretsConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprSecretsConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Secrets.DaprSecretsConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprSecretsConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Dapr.Client")
                .AddUsing("Dapr.Extensions.Configuration")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"DaprSecretsConfiguration", @class =>
                {
                    this.ApplyAppSetting("Dapr.Secrets:StoreName", "secret-store");
                    @class.Static();
                    @class.AddMethod("void", "AddDaprSecretStoreDeferred", method =>
                    {
                        method.Static()
                            .AddParameter("IConfigurationBuilder", "configBuilder", p => p.WithThisModifier())
                            .AddStatement("configBuilder.Add(new DaprSecretStoreConfigurationSourceDeferred());")
                            ;
                    });
                    @class.AddMethod("void", "LoadDaprSecretStoreDeferred", method =>
                    {

                        method.Static()
                            .AddParameter("IApplicationBuilder", "app", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            .AddStatement("var config = (IConfigurationRoot)configuration;")
                            .AddStatement("var deferredProvider = (DaprSecretsStoreProviderDeferred)config.Providers.First(x => x is DaprSecretsStoreProviderDeferred);")
                            .AddStatement("string store = configuration.GetValue<string>(\"Dapr.Secrets:StoreName\") ?? \"secret-store\" ;")
                            .AddStatement("string? descriptorsList = configuration.GetValue<string?>(\"Dapr.Secrets:Descriptors\");")
                            .AddStatement("var secretDescriptors = CreateDescriptors(descriptorsList);")
                            .AddStatement("var client = new DaprClientBuilder().Build();")
                            .AddStatement("deferredProvider.Load(client, store, secretDescriptors);")
                            ;
                    });
                    @class.AddMethod("List<DaprSecretDescriptor>?", "CreateDescriptors", method =>
                    {

                        method.Static()
                            .AddParameter("string?", "descriptorsList")
                            .AddIfStatement("string.IsNullOrWhiteSpace(descriptorsList)", body => { body.AddStatement("return null;"); })
                            .AddStatement("var result = new List<DaprSecretDescriptor>();")
                            .AddStatement("string[] descriptors = descriptorsList.Trim().Split(',');")
                            .AddForEachStatement("descriptor", "descriptors", loop =>
                            {
                                loop.AddStatement("result.Add(new DaprSecretDescriptor(descriptor.Trim()));");
                            })
                            .AddStatement("return result;")
                            ;
                    });
                    @class.AddNestedClass("DaprSecretStoreConfigurationSourceDeferred", child =>
                    {
                        child.Private()
                            .ImplementsInterface("IConfigurationSource")
                            .WithComments(@"
/// <summary>
/// This class is basically a copy of DaprSecretStoreConfigurationSource in the Dapr.Extensions.Configuration assembly.
/// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
/// because We are using SideKick we can not load the Secrets Config until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
/// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprSecretStoreConfigurationSource.cs
/// </summary>");
                        child.AddMethod("IConfigurationProvider", "Build", method =>
                        {
                            method.AddParameter("IConfigurationBuilder", "builder")
                                .AddStatement("return new DaprSecretsStoreProviderDeferred();");
                        });
                    });
                    @class.AddNestedClass("DaprSecretsStoreProviderDeferred", child =>
                    {
                        child.Private()
                            .ImplementsInterface("ConfigurationProvider")
                            .WithComments(@"
/// <summary>
/// This class is basically a copy of DaprSecretsStoreProvider in the Dapr.Extensions.Configuration assembly.
/// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
/// because We are using SideKick we can not load the Secrets Config until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
/// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprSecretsStoreProvider.cs
/// </summary>");
                        child.AddField("TimeSpan", "_sidecarWaitTimeout", f => f.PrivateReadOnly().WithAssignment("TimeSpan.FromSeconds(35)"));
                        child.AddField("bool", "_normalizeKey", f => f.PrivateReadOnly().WithAssignment("true"));
                        child.AddField("IList<string>", "_keyDelimiters", f => f.PrivateReadOnly().WithAssignment("new List<string> { \"__\" }"));
                        child.AddField("IReadOnlyDictionary<string, string>?", "_metadata", f => f.PrivateReadOnly().WithAssignment("null"));
                        child.AddConstructor();
                        child.AddMethod("string", "NormalizeKey", method =>
                        {
                            method
                            .Private()
                            .Static()
                            .AddParameter("IList<string>", "keyDelimiters")
                            .AddParameter("string", "key")
                            .AddIfStatement("keyDelimiters?.Count > 0", condition =>
                            {
                                condition.AddForEachStatement("keyDelimiter", "keyDelimiters", stmt =>
                                {
                                    stmt.AddStatement("key = key.Replace(keyDelimiter, ConfigurationPath.KeyDelimiter);");
                                });
                            })
                            .AddStatement("return key;")
                            ;
                        });
                        child.AddMethod("void", "Load", method =>
                        {
                            method.AddParameter("DaprClient", "client");
                            method.AddParameter("string", "store");
                            method.AddParameter("List<DaprSecretDescriptor>?", "secretDescriptors", p => p.WithDefaultValue("null"));
                            method.AddStatement(@"// Wait for the Dapr Sidecar to report healthy before attempting to fetch secrets.
                using (var tokenSource = new CancellationTokenSource(_sidecarWaitTimeout))
                {
                    client.WaitForSidecarAsync(tokenSource.Token).GetAwaiter().GetResult();
                }

                if (secretDescriptors != null)
                {
                    foreach (var secretDescriptor in secretDescriptors)
                    {
                        var result = client.GetSecretAsync(store, secretDescriptor.SecretName, secretDescriptor.Metadata).GetAwaiter().GetResult();

                        foreach (var key in result.Keys)
                        {
                            if (Data.ContainsKey(key))
                            {
                                throw new InvalidOperationException($""A duplicate key '{key}' was found in the secret store '{store}'. Please remove any duplicates from your secret store."");
                            }

                            Set(_normalizeKey ? NormalizeKey(_keyDelimiters, key) : key, result[key]);
                        }
                    }
                }
                else
                {
                    var result = client.GetBulkSecretAsync(store, _metadata).GetAwaiter().GetResult();
                    foreach (var key in result.Keys)
                    {
                        foreach (var secret in result[key])
                        {
                            if (Data.ContainsKey(secret.Key))
                            {
                                throw new InvalidOperationException($""A duplicate key '{secret.Key}' was found in the secret store '{store}'. Please remove any duplicates from your secret store."");
                            }

                            Set(_normalizeKey ? NormalizeKey(_keyDelimiters, secret.Key) : secret.Key, secret.Value);
                        }
                    }
                }
                OnReload();
");
                        });
                    });
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