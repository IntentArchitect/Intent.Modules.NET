using System.Text;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.SecretsManager.AwsSecretsManagerConfigurationSource", Version = "1.0")]

namespace AwsSecretsManager.Api.Configuration
{
    public class AwsSecretsManagerConfigurationProvider(string region, string secretName) : ConfigurationProvider
    {
        private const string SecretVersionStage = "AWSCURRENT";

        public override void Load()
        {
            var secretJson = GetSecret();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(secretJson)))
            {
                var tempConfig = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();
                Data = tempConfig.AsEnumerable(makePathsRelative: false)
                    .Where(kv => kv.Value is not null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
        }

        private string GetSecret()
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = SecretVersionStage
            };

            using (var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region)))
            {
                var response = client.GetSecretValueAsync(request).GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(response.SecretString))
                {
                    return response.SecretString;
                }

                using (var reader = new StreamReader(response.SecretBinary))
                {
                    var base64 = reader.ReadToEnd();
                    var bytes = Convert.FromBase64String(base64);
                    return Encoding.UTF8.GetString(bytes);
                }
            }
        }
    }

    public class AwsSecretsManagerConfigurationSource(string region, string secretName) : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AwsSecretsManagerConfigurationProvider(region, secretName);
        }
    }
}