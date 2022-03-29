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

    public class APISettings
    {
        private readonly IGroupSettings _groupSettings;

        public APISettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
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
                    "secured" => DefaultAPISecurityOptionsEnum.SecuredByDefault,
                    "unsecured" => DefaultAPISecurityOptionsEnum.UnsecuredByDefault,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSecuredByDefault()
            {
                return Value == "secured";
            }

            public bool IsUnsecuredByDefault()
            {
                return Value == "unsecured";
            }
        }

        public enum DefaultAPISecurityOptionsEnum
        {
            SecuredByDefault,
            UnsecuredByDefault,
        }
    }
}