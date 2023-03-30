using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AzureFunctions.Api
{
    public static class AzureFunctionModelStereotypeExtensions
    {
        public static AzureFunction GetAzureFunction(this AzureFunctionModel model)
        {
            var stereotype = model.GetStereotype("Azure Function");
            return stereotype != null ? new AzureFunction(stereotype) : null;
        }


        public static bool HasAzureFunction(this AzureFunctionModel model)
        {
            return model.HasStereotype("Azure Function");
        }

        public static bool TryGetAzureFunction(this AzureFunctionModel model, out AzureFunction stereotype)
        {
            if (!HasAzureFunction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureFunction(model.GetStereotype("Azure Function"));
            return true;
        }

        public class AzureFunction
        {
            private IStereotype _stereotype;

            public AzureFunction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

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

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }

            public string Connection()
            {
                return _stereotype.GetProperty<string>("Connection");
            }

            public ReturnTypeMediatypeOptions ReturnTypeMediatype()
            {
                return new ReturnTypeMediatypeOptions(_stereotype.GetProperty<string>("Return Type Mediatype"));
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Http Trigger":
                            return TypeOptionsEnum.HttpTrigger;
                        case "Service Bus Trigger":
                            return TypeOptionsEnum.ServiceBusTrigger;
                        case "Queue Trigger":
                            return TypeOptionsEnum.QueueTrigger;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsHttpTrigger()
                {
                    return Value == "Http Trigger";
                }
                public bool IsServiceBusTrigger()
                {
                    return Value == "Service Bus Trigger";
                }
                public bool IsQueueTrigger()
                {
                    return Value == "Queue Trigger";
                }
            }

            public enum TypeOptionsEnum
            {
                HttpTrigger,
                ServiceBusTrigger,
                QueueTrigger
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
                        case "PATCH":
                            return MethodOptionsEnum.PATCH;
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
                public bool IsPATCH()
                {
                    return Value == "PATCH";
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
                PATCH,
                DELETE
            }
            public class ReturnTypeMediatypeOptions
            {
                public readonly string Value;

                public ReturnTypeMediatypeOptions(string value)
                {
                    Value = value;
                }

                public ReturnTypeMediatypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return ReturnTypeMediatypeOptionsEnum.Default;
                        case "application/json":
                            return ReturnTypeMediatypeOptionsEnum.ApplicationJson;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsApplicationJson()
                {
                    return Value == "application/json";
                }
            }

            public enum ReturnTypeMediatypeOptionsEnum
            {
                Default,
                ApplicationJson
            }

            [IntentManaged(Mode.Ignore)]
            public HttpTriggerView GetHttpTriggerView()
            {
                if (!Type().IsHttpTrigger())
                {
                    return null;
                }

                return new HttpTriggerView(this.AuthorizationLevel(), this.Method(), this.Route());
            }

            [IntentManaged(Mode.Ignore)]
            public QueueDetailView GetServiceBusTriggerView()
            {
                if (!Type().IsServiceBusTrigger())
                {
                    return null;
                }

                return new QueueDetailView(this.QueueName(), this.Connection());
            }

            [IntentManaged(Mode.Ignore)]
            public QueueDetailView GetQueueTriggerView()
            {
                if (!Type().IsQueueTrigger())
                {
                    return null;
                }

                return new QueueDetailView(this.QueueName(), this.Connection());
            }

            [IntentManaged(Mode.Ignore)]
            public class HttpTriggerView
            {
                private readonly AuthorizationLevelOptions _authorizationLevel;
                private readonly MethodOptions _method;
                private readonly string _route;

                public HttpTriggerView(AuthorizationLevelOptions authorizationLevel, MethodOptions method, string route)
                {
                    _authorizationLevel = authorizationLevel;
                    _method = method;
                    _route = route;
                }

                public AuthorizationLevelOptions AuthorizationLevel()
                {
                    return _authorizationLevel;
                }

                public MethodOptions Method()
                {
                    return _method;
                }

                public string Route()
                {
                    return _route;
                }
            }

            [IntentManaged(Mode.Ignore)]
            public class QueueDetailView
            {
                private readonly string _queueName;
                private readonly string _connection;

                public QueueDetailView(string queueName, string connection)
                {
                    _queueName = queueName;
                    _connection = connection;
                }

                public string QueueName()
                {
                    return _queueName;
                }

                public string Connection()
                {
                    return _connection;
                }
            }
        }
    }
}