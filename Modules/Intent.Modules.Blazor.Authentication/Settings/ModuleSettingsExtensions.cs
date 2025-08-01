using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Settings
{

    public static class BlazorExtensions
    {
        [IntentIgnore]
        public static AuthenticationOptions Authentication(this Intent.Modules.Blazor.Settings.Blazor groupSettings) => new AuthenticationOptions(groupSettings.GetSetting("5ec4a775-6208-405b-b66f-0dd5c6e591bb")?.Value);

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
                    "aspnetcore-identity" => AuthenticationOptionsEnum.AspnetcoreIdentity,
                    "jwt" => AuthenticationOptionsEnum.Jwt,
                    "oidc" => AuthenticationOptionsEnum.Oidc,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsAspnetcoreIdentity()
            {
                return Value == "aspnetcore-identity";
            }

            public bool IsJwt()
            {
                return Value == "jwt";
            }

            public bool IsOidc()
            {
                return Value == "oidc";
            }
        }

        public enum AuthenticationOptionsEnum
        {
            AspnetcoreIdentity,
            Jwt,
            Oidc,
        }
    }
}