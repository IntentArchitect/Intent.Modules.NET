using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.SecretsManager.AwsSecretsManagerConfiguration", Version = "1.0")]

namespace AwsSecretsManager.Api.Configuration
{
    public static class AwsSecretsManagerConfiguration
    {
        public static void ConfigureAwsSecretsManager(this IConfigurationBuilder builder, IConfiguration configuration)
        {
            var options = new AwsSecretsManagerOptions();
            configuration.GetSection("SecretsManager").Bind(options);

            foreach (var secret in options.Secrets)
            {
                builder.Add(new AwsSecretsManagerConfigurationSource(secret.Region, secret.SecretName));
            }
        }
    }
}