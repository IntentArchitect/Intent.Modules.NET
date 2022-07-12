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

            public MethodOptions Method()
            {
                return new MethodOptions(_stereotype.GetProperty<string>("Method"));
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
            public class MethodOptions
            {
                public readonly string Value;

                public MethodOptions(string value)
                {
                    Value = value;
                }

                public MethodOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "GET":
                            return MethodOptionsEnum.GET;
                        case "POST":
                            return MethodOptionsEnum.POST;
                        case "PUT":
                            return MethodOptionsEnum.PUT;
                        case "DELETE":
                            return MethodOptionsEnum.DELETE;
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

            public enum MethodOptionsEnum
            {
                GET,
                POST,
                PUT,
                DELETE
            }
        }

    }
}