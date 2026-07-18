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
    public static class UserInterfacePackageModelStereotypeExtensions
    {
        public static Security GetSecurity(this UserInterfacePackageModel model)
        {
            var stereotype = model.GetStereotype(Security.DefinitionId);
            return stereotype != null ? new Security(stereotype) : null;
        }


        public static bool HasSecurity(this UserInterfacePackageModel model)
        {
            return model.HasStereotype(Security.DefinitionId);
        }

        public static bool TryGetSecurity(this UserInterfacePackageModel model, out Security stereotype)
        {
            if (!HasSecurity(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Security(model.GetStereotype(Security.DefinitionId));
            return true;
        }

        public class Security
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "472c2df3-e807-4cf6-89b1-8213cd485c26";

            public Security(IStereotype stereotype)
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
                        case "None":
                            return AuthenticationOptionsEnum.None;
                        case "Bearer Token (JWT)":
                            return AuthenticationOptionsEnum.BearerTokenJWT;
                        case "Single Sign-On (OpenID Connect)":
                            return AuthenticationOptionsEnum.SingleSignOnOpenIDConnect;
                        case "Built-in Login (ASP.NET Identity)":
                            return AuthenticationOptionsEnum.BuiltInLoginASPNETIdentity;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsBearerTokenJWT()
                {
                    return Value == "Bearer Token (JWT)";
                }
                public bool IsSingleSignOnOpenIDConnect()
                {
                    return Value == "Single Sign-On (OpenID Connect)";
                }
                public bool IsBuiltInLoginASPNETIdentity()
                {
                    return Value == "Built-in Login (ASP.NET Identity)";
                }
            }

            public enum AuthenticationOptionsEnum
            {
                None,
                BearerTokenJWT,
                SingleSignOnOpenIDConnect,
                BuiltInLoginASPNETIdentity
            }
        }

    }
}