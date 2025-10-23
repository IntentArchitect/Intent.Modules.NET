using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.SecretsManager.AwsSecretsManagerOptions", Version = "1.0")]

namespace AwsSecretsManager.Api.Configuration
{
    public class AwsSecretsManagerOptions
    {
        public bool Enabled { get; set; }
        public List<AwsSecretsManagerSecret> Secrets { get; set; } = new List<AwsSecretsManagerSecret>();
    }

    public class AwsSecretsManagerSecret
    {
        public string Region { get; set; }
        public string SecretName { get; set; }
    }
}