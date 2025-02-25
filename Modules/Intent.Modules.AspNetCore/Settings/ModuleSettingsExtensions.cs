using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ASPNETCoreSettings GetASPNETCoreSettings(this IApplicationSettingsProvider settings)
        {
            return new ASPNETCoreSettings(settings.GetGroup("9677c813-b334-4b0d-be2c-3b149f1f6ae8"));
        }
    }

    public class ASPNETCoreSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ASPNETCoreSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }

        public bool EnableHTTPSRedirect() => bool.TryParse(_groupSettings.GetSetting("e715d518-88c0-4688-91c4-01ec28ce43a2")?.Value.ToPascalCase(), out var result) && result;
    }
}