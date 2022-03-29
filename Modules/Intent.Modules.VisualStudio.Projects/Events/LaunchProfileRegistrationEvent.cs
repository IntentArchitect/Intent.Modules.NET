using System;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Constants
{
    /// <summary>
    /// Obsolete. Use <see cref="LaunchProfileRegistrationRequest"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static class LaunchProfileRegistrationEvent
    {
        public const string EventId = nameof(LaunchProfileRegistrationEvent);
        public const string ProfileNameKey = "profileName";
        public const string CommandNameKey = "commandName";
        public const string LaunchBrowserKey = "launchBrowser";
        public const string LaunchUrlKey = "launchUrl";
        public const string ApplicationUrl = "applicationUrl";
        public const string UseSSL = "useSSL";
        public const string PublishAllPorts = "publishAllPorts";
    }
}