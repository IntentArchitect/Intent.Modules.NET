using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Blazor.Authentication.Api
{
    public static class SecurityConfigurationModelStereotypeExtensions
    {
        public static SecurityType GetSecurityType(this SecurityConfigurationModel model)
        {
            var stereotype = model.GetStereotype(SecurityType.DefinitionId);
            return stereotype != null ? new SecurityType(stereotype) : null;
        }


        public static bool HasSecurityType(this SecurityConfigurationModel model)
        {
            return model.HasStereotype(SecurityType.DefinitionId);
        }

        public static bool TryGetSecurityType(this SecurityConfigurationModel model, out SecurityType stereotype)
        {
            if (!HasSecurityType(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SecurityType(model.GetStereotype(SecurityType.DefinitionId));
            return true;
        }

        public class SecurityType
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "472c2df3-e807-4cf6-89b1-8213cd485c26";

            public SecurityType(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public AuthenticationOptions Authentication()
            {
                return new AuthenticationOptions(_stereotype.GetProperty<string>("Authentication"));
            }

            public class AuthenticationOptions
            {
                public readonly string Value;

                public AuthenticationOptions(string value)
                {
                    Value = value;
                }

                public AuthenticationOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "ASP.NET Core Identity":
                            return AuthenticationOptionsEnum.ASPNETCoreIdentity;
                        case "JWT":
                            return AuthenticationOptionsEnum.JWT;
                        case "OIDC":
                            return AuthenticationOptionsEnum.OIDC;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsASPNETCoreIdentity()
                {
                    return Value == "ASP.NET Core Identity";
                }
                public bool IsJWT()
                {
                    return Value == "JWT";
                }
                public bool IsOIDC()
                {
                    return Value == "OIDC";
                }
            }

            public enum AuthenticationOptionsEnum
            {
                ASPNETCoreIdentity,
                JWT,
                OIDC
            }
        }

    }
}