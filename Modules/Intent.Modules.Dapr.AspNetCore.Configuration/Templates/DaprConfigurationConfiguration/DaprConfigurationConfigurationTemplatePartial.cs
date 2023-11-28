using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

namespace Intent.Modules.Dapr.AspNetCore.Configuration.Templates.DaprConfigurationConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprConfigurationConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Configuration.DaprConfigurationConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprConfigurationConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Dapr.Client")
                .AddUsing("Dapr.Extensions.Configuration")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"DaprConfigurationConfiguration", @class =>
                {
                    this.ApplyAppSetting("Dapr.Configuration:StoreName", "configuration-store");
                    this.ApplyAppSetting("Dapr.Configuration:Keys", "{Comma separated list of config keys to load}");
                    @class.Static();
                    @class.AddMethod("void", "AddDaprConfigurationStoreDeferred", method =>
                    {
                        method.Static()
                            .AddParameter("IConfigurationBuilder", "configBuilder", p => p.WithThisModifier())
                            .AddStatement("configBuilder.Add(new DaprConfigurationStoreSourceDeferred());")
                            ;
                    });
                    @class.AddMethod("void", "LoadDaprConfigurationStoreDeferred", method =>
                    {

                        method.Static()
                            .AddParameter("IApplicationBuilder", "app", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            .AddStatement("var config = (IConfigurationRoot)configuration;")
                            .AddStatement("var deferredProvider = (DaprConfigurationStoreProviderDeferred)config.Providers.First(x => x is DaprConfigurationStoreProviderDeferred);")
                            .AddStatement("string store = configuration.GetValue<string>(\"Dapr.Configuration:StoreName\") ?? \"configuration-store\" ;")
                            .AddStatement("string keysList = configuration.GetValue<string>(\"Dapr.Configuration:Keys\");")
                            .AddStatement("var keys = CreateKeys(keysList);")
                            .AddStatement("var client = new DaprClientBuilder().Build();")
                            .AddStatement("deferredProvider.Load(client, store, keys);")
                            ;
                    });
                    @class.AddMethod("List<string>", "CreateKeys", method =>
                    {

                        method.Static()
                            .AddParameter("string", "keysStr")
                            .AddIfStatement("string.IsNullOrWhiteSpace(keysStr)", body => { body.AddStatement("throw new ArgumentException(\"Dapr.Configuration:Keys not configured.\");"); })
                            .AddStatement("var result = new List<string>();")
                            .AddStatement("string[] keys = keysStr.Trim().Split(',');")
                            .AddForEachStatement("key", "keys", loop =>
                            {
                                loop.AddStatement("result.Add(key.Trim());");
                            })
                            .AddStatement("return result;")
                            ;
                    });
                    @class.AddNestedClass("DaprConfigurationStoreSourceDeferred", child =>
                    {
                        child.Private()
                            .ImplementsInterface("IConfigurationSource")
                            .WithComments(@"
/// <summary>
/// This class is basically a copy of DaprConfigurationStoreSource in the Dapr.Extensions.Configuration assembly.
/// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
/// because We are using SideKick we can not load the Dapr Configurations until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
/// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprConfigurationStoreSource.cs
/// </summary>");
                        child.AddMethod("IConfigurationProvider", "Build", method =>
                        {
                            method.AddParameter("IConfigurationBuilder", "builder")
                                .AddStatement("return new DaprConfigurationStoreProviderDeferred();");
                        });
                    });
                    @class.AddNestedClass("DaprConfigurationStoreProviderDeferred", child =>
                    {
                        child.Private()
                            .ImplementsInterface("ConfigurationProvider")
                            .WithComments(@"
/// <summary>
/// This class is basically a copy of DaprConfigurationStoreProvider in the Dapr.Extensions.Configuration assembly.
/// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
/// because We are using SideKick we can not load the Dapr Configurations until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
/// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprConfigurationStoreProvider.cs
/// </summary>");
                        child.AddField("TimeSpan", "_sidecarWaitTimeout", f => f.PrivateReadOnly().WithAssignment("TimeSpan.FromSeconds(35)"))
                            .AddField("bool", "_normalizeKey", f => f.PrivateReadOnly().WithAssignment("true"))
                            .AddField("IList<string>", "_keyDelimiters", f => f.PrivateReadOnly().WithAssignment("new List<string> { \"__\" }"))
                            .AddField("IReadOnlyDictionary<string, string>?", "_metadata", f => f.PrivateReadOnly().WithAssignment("null"))
                            .AddField("CancellationTokenSource", "_cts", f => f.PrivateReadOnly().WithAssignment("new CancellationTokenSource()"))
                            .AddField("Task", "_subscribeTask", f => f.Private().WithAssignment("Task.CompletedTask"))
                            .AddField("bool", "_isStreaming", f => f.PrivateReadOnly().WithAssignment("true"));

                        child.AddConstructor();
                        child.AddMethod("void", "Dispose", method =>
                        {
                            method.AddStatement("_cts.Cancel();")
                            ;
                        });
                        child.AddMethod("void", "Load", method =>
                        {
                            method.AddParameter("DaprClient", "daprClient");
                            method.AddParameter("string", "store");
                            method.AddParameter("List<string>", "keys");
                            method.AddStatement(@"                // Wait for the sidecar to become available.
                using (var tokenSource = new CancellationTokenSource(_sidecarWaitTimeout))
                {
                    daprClient.WaitForSidecarAsync(tokenSource.Token).GetAwaiter().GetResult();
                }

                if (_isStreaming)
                {
                    _subscribeTask = Task.Run(async () =>
                    {
                        while (!_cts.Token.IsCancellationRequested)
                        {
                            var id = string.Empty;
                            try
                            {
                                var subscribeConfigurationResponse = await daprClient.SubscribeConfiguration(store, keys, _metadata, _cts.Token);
                                await foreach (var items in subscribeConfigurationResponse.Source.WithCancellation(_cts.Token))
                                {
                                    var data = new Dictionary<string, string>(Data);
                                    foreach (var item in items)
                                    {
                                        id = subscribeConfigurationResponse.Id;
                                        data[item.Key] = item.Value.Value;
                                    }
                                    Data = data;
                                    // Whenever we get an update, make sure to update the reloadToken.
                                    OnReload();
                                }
                            }
                            catch (Exception)
                            {
                                // If we catch an exception, try and cancel the subscription so we can connect again.
                                if (!string.IsNullOrEmpty(id))
                                {
                                    daprClient.UnsubscribeConfiguration(store, id).GetAwaiter().GetResult(); ;
                                }
                            }
                        }
                    });
                }
                else
                {
                    // We don't need to worry about ReloadTokens here because it is a constant response.
                    var getConfigurationResponse = daprClient.GetConfiguration(store, keys, _metadata, _cts.Token).GetAwaiter().GetResult(); ;
                    foreach (var item in getConfigurationResponse.Items)
                    {
                        Set(item.Key, item.Value.Value);
                    }
                    OnReload();
                }
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