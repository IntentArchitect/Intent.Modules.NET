using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static IdentityServerSettings GetIdentityServerSettings(this IApplicationSettingsProvider settings)
        {
            return new IdentityServerSettings(settings.GetGroup("2540e7d3-58b4-4e0d-9187-f614bf834837"));
        }
    }

    public class IdentityServerSettings
    {
        private readonly IGroupSettings _groupSettings;

        public IdentityServerSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public bool CreateTestUsers() => bool.TryParse(_groupSettings.GetSetting("e31ca033-a677-4674-aeca-126633e3e097")?.Value.ToPascalCase(), out var result) && result;
    }
}