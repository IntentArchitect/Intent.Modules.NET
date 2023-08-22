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
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Secrets.DaprSecretsConfiguration", Version = "1.0")]

namespace CleanArchitecture.Dapr.Api.Configuration
{
    public static class DaprSecretsConfiguration
    {
        public static void AddDaprSecretStoreDeferred(this IConfigurationBuilder configBuilder)
        {
            configBuilder.Add(new DaprSecretStoreConfigurationSourceDeferred());
        }

        public static void LoadDaprSecretStoreDeferred(this IApplicationBuilder app, IConfiguration configuration)
        {
            var config = (IConfigurationRoot)configuration;
            var deferredProvider = (DaprSecretsStoreProviderDeferred)config.Providers.First(x => x is DaprSecretsStoreProviderDeferred);
            string store = configuration.GetValue<string>("Dapr.Secrets:StoreName") ?? "secret-store";
            string? descriptorsList = configuration.GetValue<string?>("Dapr.Secrets:Descriptors");
            var secretDescriptors = CreateDescriptors(descriptorsList);
            var client = new DaprClientBuilder().Build();
            deferredProvider.Load(client, store, secretDescriptors);
        }

        public static List<DaprSecretDescriptor>? CreateDescriptors(string? descriptorsList)
        {
            if (string.IsNullOrWhiteSpace(descriptorsList))
            {
                return null;
            }
            var result = new List<DaprSecretDescriptor>();
            string[] descriptors = descriptorsList.Trim().Split(',');

            foreach (var descriptor in descriptors)
            {
                result.Add(new DaprSecretDescriptor(descriptor.Trim()));
            }
            return result;
        }

        /// <summary>
        /// This class is basically a copy of DaprSecretStoreConfigurationSource in the Dapr.Extensions.Configuration assembly.
        /// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
        /// because We are using SideKick we can not load the Secrets Config until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
        /// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprSecretStoreConfigurationSource.cs
        /// </summary>
        private class DaprSecretStoreConfigurationSourceDeferred : IConfigurationSource
        {
            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new DaprSecretsStoreProviderDeferred();
            }
        }
        /// <summary>
        /// This class is basically a copy of DaprSecretsStoreProvider in the Dapr.Extensions.Configuration assembly.
        /// A standard Dapr implementation would load configuration in CreateHostBuilder (Program.cs)
        /// because We are using SideKick we can not load the Secrets Config until the SideCar is ready which happened after ServicesConfiguration (StartUp.cs).
        /// https://github.com/dapr/dotnet-sdk/blob/master/src/Dapr.Extensions.Configuration/DaprSecretsStoreProvider.cs
        /// </summary>
        private class DaprSecretsStoreProviderDeferred : ConfigurationProvider
        {
            private readonly TimeSpan _sidecarWaitTimeout = TimeSpan.FromSeconds(35);
            private readonly bool _normalizeKey = true;
            private readonly IList<string> _keyDelimiters = new List<string> { "__" };
            private readonly IReadOnlyDictionary<string, string>? _metadata = null;

            public DaprSecretsStoreProviderDeferred()
            {
            }

            private static string NormalizeKey(IList<string> keyDelimiters, string key)
            {
                if (keyDelimiters?.Count > 0)
                {
                    foreach (var keyDelimiter in keyDelimiters)
                    {
                        key = key.Replace(keyDelimiter, ConfigurationPath.KeyDelimiter);
                    }
                }
                return key;
            }

            public void Load(DaprClient client, string store, List<DaprSecretDescriptor>? secretDescriptors = null)
            {
                // Wait for the Dapr Sidecar to report healthy before attempting to fetch secrets.
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
                                throw new InvalidOperationException($"A duplicate key '{key}' was found in the secret store '{store}'. Please remove any duplicates from your secret store.");
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
                                throw new InvalidOperationException($"A duplicate key '{secret.Key}' was found in the secret store '{store}'. Please remove any duplicates from your secret store.");
                            }

                            Set(_normalizeKey ? NormalizeKey(_keyDelimiters, secret.Key) : secret.Key, secret.Value);
                        }
                    }
                }
                OnReload();
            }
        }
    }
}