using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Configuration.DaprConfigurationConfiguration", Version = "1.0")]

namespace CleanArchitecture.Dapr.Api.Configuration
{
    public static class DaprConfigurationConfiguration
    {
        public static void AddDaprConfigurationStoreDeferred(this IConfigurationBuilder configBuilder)
        {
            configBuilder.Add(new DaprConfigurationStoreSourceDeferred());
        }

        public static void LoadDaprConfigurationStoreDeferred(this IApplicationBuilder app, IConfiguration configuration)
        {
            var config = (IConfigurationRoot)configuration;
            var deferredProvider = (DaprConfigurationStoreProviderDeferred)config.Providers.First(x => x is DaprConfigurationStoreProviderDeferred);
            string store = configuration.GetValue<string>("Dapr.Configuration:StoreName") ?? "configuration-store";
            string keysList = configuration.GetValue<string>("Dapr.Configuration:Keys");
            var keys = CreateKeys(keysList);
            var client = new DaprClientBuilder().Build();
            deferredProvider.Load(client, store, keys);
        }

        public static List<string> CreateKeys(string keysStr)
        {
            if (string.IsNullOrWhiteSpace(keysStr))
            {
                throw new ArgumentException("Dapr.Configuration:Keys not configured.");
            }
            var result = new List<string>();
            string[] keys = keysStr.Trim().Split(',');

            foreach (var key in keys)
            {
                result.Add(key.Trim());
            }
            return result;
        }

        /// <summary>
        /// This class is basically a copy of DaprConfigurationStoreSource in the Dapr.Extensions.Configuration assembly.
        /// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
        /// because We are using SideKick we can not load the Dapr Configurations until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
        /// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprConfigurationStoreSource.cs
        /// </summary>
        private class DaprConfigurationStoreSourceDeferred : IConfigurationSource
        {
            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new DaprConfigurationStoreProviderDeferred();
            }
        }
        /// <summary>
        /// This class is basically a copy of DaprConfigurationStoreProvider in the Dapr.Extensions.Configuration assembly.
        /// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
        /// because We are using SideKick we can not load the Dapr Configurations until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
        /// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprConfigurationStoreProvider.cs
        /// </summary>
        private class DaprConfigurationStoreProviderDeferred : ConfigurationProvider
        {
            private readonly TimeSpan _sidecarWaitTimeout = TimeSpan.FromSeconds(35);
            private readonly bool _normalizeKey = true;
            private readonly IList<string> _keyDelimiters = new List<string> { "__" };
            private readonly IReadOnlyDictionary<string, string>? _metadata = null;
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();
            private Task _subscribeTask = Task.CompletedTask;
            private readonly bool _isStreaming = true;

            public DaprConfigurationStoreProviderDeferred()
            {
            }

            public void Dispose()
            {
                _cts.Cancel();
            }

            public void Load(DaprClient daprClient, string store, List<string> keys)
            {
                // Wait for the sidecar to become available.
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
            }
        }
    }
}