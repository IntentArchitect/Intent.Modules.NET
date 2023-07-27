using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Settings;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Security.Settings
{

    public static class SwaggerSettingsExtensions
    {
        public static AuthenticationOptions Authentication(this SwaggerSettings groupSettings) => new AuthenticationOptions(groupSettings.GetSetting("64312cd3-8742-4fa9-b42f-8e27d66407b1")?.Value);

        public class AuthenticationOptions
        {
            public readonly string Value;

            public AuthenticationOptions(string value)
            {
                Value = value;
            }

            public AuthenticationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "Bearer" => AuthenticationOptionsEnum.Bearer,
                    "Implicit" => AuthenticationOptionsEnum.Implicit,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsBearer()
            {
                return Value == "Bearer";
            }

            public bool IsImplicit()
            {
                return Value == "Implicit";
            }
        }

        public enum AuthenticationOptionsEnum
        {
            Bearer,
            Implicit,
        }
    }
}