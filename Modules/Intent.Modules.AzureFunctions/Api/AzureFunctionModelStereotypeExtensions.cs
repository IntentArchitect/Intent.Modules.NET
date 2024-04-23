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
            var stereotype = model.GetStereotype("7c1128f6-fdef-4bf9-8f15-acb54b5bfa89");
            return stereotype != null ? new AzureFunction(stereotype) : null;
        }


        public static bool HasAzureFunction(this AzureFunctionModel model)
        {
            return model.HasStereotype("7c1128f6-fdef-4bf9-8f15-acb54b5bfa89");
        }

        public static bool TryGetAzureFunction(this AzureFunctionModel model, out AzureFunction stereotype)
        {
            if (!HasAzureFunction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureFunction(model.GetStereotype("7c1128f6-fdef-4bf9-8f15-acb54b5bfa89"));
            return true;
        }

        public static CosmosDBTrigger GetCosmosDBTrigger(this AzureFunctionModel model)
        {
            var stereotype = model.GetStereotype("78edaf9d-bc43-4792-b483-408fcd630261");
            return stereotype != null ? new CosmosDBTrigger(stereotype) : null;
        }


        public static bool HasCosmosDBTrigger(this AzureFunctionModel model)
        {
            return model.HasStereotype("78edaf9d-bc43-4792-b483-408fcd630261");
        }

        public static bool TryGetCosmosDBTrigger(this AzureFunctionModel model, out CosmosDBTrigger stereotype)
        {
            if (!HasCosmosDBTrigger(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CosmosDBTrigger(model.GetStereotype("78edaf9d-bc43-4792-b483-408fcd630261"));
            return true;
        }

        public static QueueOutputBinding GetQueueOutputBinding(this AzureFunctionModel model)
        {
            var stereotype = model.GetStereotype("ec293aa6-7120-4870-800f-7db01391376f");
            return stereotype != null ? new QueueOutputBinding(stereotype) : null;
        }

        public static IReadOnlyCollection<QueueOutputBinding> GetQueueOutputBindings(this AzureFunctionModel model)
        {
            var stereotypes = model
                .GetStereotypes("ec293aa6-7120-4870-800f-7db01391376f")
                .Select(stereotype => new QueueOutputBinding(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasQueueOutputBinding(this AzureFunctionModel model)
        {
            return model.HasStereotype("ec293aa6-7120-4870-800f-7db01391376f");
        }

        public static bool TryGetQueueOutputBinding(this AzureFunctionModel model, out QueueOutputBinding stereotype)
        {
            if (!HasQueueOutputBinding(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new QueueOutputBinding(model.GetStereotype("ec293aa6-7120-4870-800f-7db01391376f"));
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
                        case "RabbitMQ Trigger":
                            return TriggerOptionsEnum.RabbitMQTrigger;
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
                public bool IsRabbitMQTrigger()
                {
                    return Value == "RabbitMQ Trigger";
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
                CosmosDBTrigger,
                RabbitMQTrigger
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

        public class CosmosDBTrigger
        {
            private IStereotype _stereotype;

            public CosmosDBTrigger(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Connection()
            {
                return _stereotype.GetProperty<string>("Connection");
            }

            public string ContainerName()
            {
                return _stereotype.GetProperty<string>("Container Name");
            }

            public string DatabaseName()
            {
                return _stereotype.GetProperty<string>("Database Name");
            }

            public string LeaseConnection()
            {
                return _stereotype.GetProperty<string>("Lease Connection");
            }

            public string LeaseDatabaseName()
            {
                return _stereotype.GetProperty<string>("Lease Database Name");
            }

            public string LeaseContainerName()
            {
                return _stereotype.GetProperty<string>("Lease Container Name");
            }

            public bool CreateLeaseContainerIfNotExists()
            {
                return _stereotype.GetProperty<bool>("Create Lease Container If not exists");
            }

            public int? LeasesContainerThroughput()
            {
                return _stereotype.GetProperty<int?>("Leases Container Throughput");
            }

            public string LeaseContainerPrefix()
            {
                return _stereotype.GetProperty<string>("Lease Container Prefix");
            }

        }

        public class QueueOutputBinding
        {
            private IStereotype _stereotype;

            public QueueOutputBinding(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }
        }
    }
}