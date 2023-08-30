using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AzureFunctions.Api
{
    public static class QueryModelStereotypeExtensions
    {
        public static AzureFunction GetAzureFunction(this QueryModel model)
        {
            var stereotype = model.GetStereotype("Azure Function");
            return stereotype != null ? new AzureFunction(stereotype) : null;
        }


        public static bool HasAzureFunction(this QueryModel model)
        {
            return model.HasStereotype("Azure Function");
        }

        public static bool TryGetAzureFunction(this QueryModel model, out AzureFunction stereotype)
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

            public TriggerOptions Trigger()
            {
                return new TriggerOptions(_stereotype.GetProperty<string>("Trigger"));
            }

            public AuthorizationLevelOptions AuthorizationLevel()
            {
                return new AuthorizationLevelOptions(_stereotype.GetProperty<string>("Authorization Level"));
            }

            public MethodOptions Method()
            {
                return new MethodOptions(_stereotype.GetProperty<string>("Method"));
            }

            public bool IncludeMessageEnvelope()
            {
                return _stereotype.GetProperty<bool>("Include Message Envelope");
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

            public string ScheduleExpression()
            {
                return _stereotype.GetProperty<string>("Schedule Expression");
            }

            public string EventHubName()
            {
                return _stereotype.GetProperty<string>("EventHub Name");
            }

            public class TriggerOptions
            {
                public readonly string Value;

                public TriggerOptions(string value)
                {
                    Value = value;
                }

                public TriggerOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Http Trigger":
                            return TriggerOptionsEnum.HttpTrigger;
                        case "Service Bus Trigger":
                            return TriggerOptionsEnum.ServiceBusTrigger;
                        case "Queue Trigger":
                            return TriggerOptionsEnum.QueueTrigger;
                        case "Timer Trigger":
                            return TriggerOptionsEnum.TimerTrigger;
                        case "EventHub Trigger":
                            return TriggerOptionsEnum.EventHubTrigger;
                        case "Manual Trigger":
                            return TriggerOptionsEnum.ManualTrigger;
                        case "Cosmos DB Trigger":
                            return TriggerOptionsEnum.CosmosDBTrigger;
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
                public bool IsTimerTrigger()
                {
                    return Value == "Timer Trigger";
                }
                public bool IsEventHubTrigger()
                {
                    return Value == "EventHub Trigger";
                }
                public bool IsManualTrigger()
                {
                    return Value == "Manual Trigger";
                }
                public bool IsCosmosDBTrigger()
                {
                    return Value == "Cosmos DB Trigger";
                }
            }

            public enum TriggerOptionsEnum
            {
                HttpTrigger,
                ServiceBusTrigger,
                QueueTrigger,
                TimerTrigger,
                EventHubTrigger,
                ManualTrigger,
                CosmosDBTrigger
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
        }

    }
}