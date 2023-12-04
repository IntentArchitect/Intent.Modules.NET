using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static APISettings GetAPISettings(this IApplicationSettingsProvider settings)
        {
            return new APISettings(settings.GetGroup("4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb"));
        }
    }

    public class APISettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public APISettings(IGroupSettings groupSettings)
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

        public DefaultAPISecurityOptions DefaultAPISecurity() => new DefaultAPISecurityOptions(_groupSettings.GetSetting("061a559a-0d54-4eb1-8c70-ed0baa238a59")?.Value);

        public class DefaultAPISecurityOptions
        {
            public readonly string Value;

            public DefaultAPISecurityOptions(string value)
            {
                Value = value;
            }

            public DefaultAPISecurityOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "secured" => DefaultAPISecurityOptionsEnum.Secured,
                    "unsecured" => DefaultAPISecurityOptionsEnum.Unsecured,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSecured()
            {
                return Value == "secured";
            }

            public bool IsUnsecured()
            {
                return Value == "unsecured";
            }
        }

        public enum DefaultAPISecurityOptionsEnum
        {
            Secured,
            Unsecured,
        }

        public string DefaultAPIRoutePrefix() => _groupSettings.GetSetting("07c796c5-ff1c-4fb4-b9a4-bc9000439fec")?.Value;

        public bool SerializeEnumsAsStrings() => bool.TryParse(_groupSettings.GetSetting("97f3a1e3-2455-41e8-b28e-709f2db04230")?.Value.ToPascalCase(), out var result) && result;

        public bool OnSerializationIgnoreJSONReferenceCycles() => bool.TryParse(_groupSettings.GetSetting("905d0eaa-b32c-4b3d-a6a5-05de63cb5f71")?.Value.ToPascalCase(), out var result) && result;
    }
}