using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.SignalR.Api
{
    public static class SendMessageModelStereotypeExtensions
    {
        public static HubSendMessageSettings GetHubSendMessageSettings(this SendMessageModel model)
        {
            var stereotype = model.GetStereotype("a711f094-48c5-4011-ae90-666e83bd959f");
            return stereotype != null ? new HubSendMessageSettings(stereotype) : null;
        }


        public static bool HasHubSendMessageSettings(this SendMessageModel model)
        {
            return model.HasStereotype("a711f094-48c5-4011-ae90-666e83bd959f");
        }

        public static bool TryGetHubSendMessageSettings(this SendMessageModel model, out HubSendMessageSettings stereotype)
        {
            if (!HasHubSendMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HubSendMessageSettings(model.GetStereotype("a711f094-48c5-4011-ae90-666e83bd959f"));
            return true;
        }

        public class HubSendMessageSettings
        {
            private IStereotype _stereotype;

            public HubSendMessageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TargetClientsOptions TargetClients()
            {
                return new TargetClientsOptions(_stereotype.GetProperty<string>("Target Clients"));
            }

            public class TargetClientsOptions
            {
                public readonly string Value;

                public TargetClientsOptions(string value)
                {
                    Value = value;
                }

                public TargetClientsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "All":
                            return TargetClientsOptionsEnum.All;
                        case "User":
                            return TargetClientsOptionsEnum.User;
                        case "Users":
                            return TargetClientsOptionsEnum.Users;
                        case "Group":
                            return TargetClientsOptionsEnum.Group;
                        case "Groups":
                            return TargetClientsOptionsEnum.Groups;
                        case "Client":
                            return TargetClientsOptionsEnum.Client;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAll()
                {
                    return Value == "All";
                }
                public bool IsUser()
                {
                    return Value == "User";
                }
                public bool IsUsers()
                {
                    return Value == "Users";
                }
                public bool IsGroup()
                {
                    return Value == "Group";
                }
                public bool IsGroups()
                {
                    return Value == "Groups";
                }
                public bool IsClient()
                {
                    return Value == "Client";
                }
            }

            public enum TargetClientsOptionsEnum
            {
                All,
                User,
                Users,
                Group,
                Groups,
                Client
            }
        }

    }
}