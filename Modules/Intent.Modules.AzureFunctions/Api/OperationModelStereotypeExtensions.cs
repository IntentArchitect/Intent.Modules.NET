using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AzureFunctions.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static HttpTrigger GetHttpTrigger(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Http Trigger");
            return stereotype != null ? new HttpTrigger(stereotype) : null;
        }


        public static bool HasHttpTrigger(this OperationModel model)
        {
            return model.HasStereotype("Http Trigger");
        }


        public class HttpTrigger
        {
            private IStereotype _stereotype;

            public HttpTrigger(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public AuthorizationLevelOptions AuthorizationLevel()
            {
                return new AuthorizationLevelOptions(_stereotype.GetProperty<string>("Authorization Level"));
            }

            public MethodsOptions[] Methods()
            {
                return _stereotype.GetProperty<string[]>("Methods")?.Select(x => new MethodsOptions(x)).ToArray() ?? new MethodsOptions[0];
            }

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

            public class AuthorizationLevelOptions
            {
                public readonly string Value;

                public AuthorizationLevelOptions(string value)
                {
                    Value = value;
                }

                public AuthorizationLevelOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Anonymous":
                            return AuthorizationLevelOptionsEnum.Anonymous;
                        case "User":
                            return AuthorizationLevelOptionsEnum.User;
                        case "Function":
                            return AuthorizationLevelOptionsEnum.Function;
                        case "System":
                            return AuthorizationLevelOptionsEnum.System;
                        case "Admin":
                            return AuthorizationLevelOptionsEnum.Admin;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAnonymous()
                {
                    return Value == "Anonymous";
                }
                public bool IsUser()
                {
                    return Value == "User";
                }
                public bool IsFunction()
                {
                    return Value == "Function";
                }
                public bool IsSystem()
                {
                    return Value == "System";
                }
                public bool IsAdmin()
                {
                    return Value == "Admin";
                }
            }

            public enum AuthorizationLevelOptionsEnum
            {
                Anonymous,
                User,
                Function,
                System,
                Admin
            }
            public class MethodsOptions
            {
                public readonly string Value;

                public MethodsOptions(string value)
                {
                    Value = value;
                }

                public MethodsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "GET":
                            return MethodsOptionsEnum.GET;
                        case "POST":
                            return MethodsOptionsEnum.POST;
                        case "PUT":
                            return MethodsOptionsEnum.PUT;
                        case "DELETE":
                            return MethodsOptionsEnum.DELETE;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsGET()
                {
                    return Value == "GET";
                }
                public bool IsPOST()
                {
                    return Value == "POST";
                }
                public bool IsPUT()
                {
                    return Value == "PUT";
                }
                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
            }

            public enum MethodsOptionsEnum
            {
                GET,
                POST,
                PUT,
                DELETE
            }
        }

    }
}