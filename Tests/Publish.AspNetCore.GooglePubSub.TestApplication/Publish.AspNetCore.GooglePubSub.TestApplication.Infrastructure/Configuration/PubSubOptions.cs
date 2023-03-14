using Google.Api.Gax;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.ConfigurationTemplates.PubSubOptions", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Configuration
{
    public class PubSubOptions
    {
        public bool UseMetadataServer { get; set; }
        public bool ShouldSetupCloudResources { get; set; }
        public string ProjectId { get; set; }
        public bool UsePubSubEmulator { get; set; }
        public bool ShouldAuthorizePushNotification { get; set; }
        public string VerificationToken { get; set; }

        public EmulatorDetection GetEmulatorDetectionMode()
        {
            return UsePubSubEmulator ? EmulatorDetection.EmulatorOnly : EmulatorDetection.None;
        }

        public bool HasVerificationToken()
        {
            return !string.IsNullOrWhiteSpace(VerificationToken);
        }
    }
}