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
    public static class PublishMessageModelStereotypeExtensions
    {
        public static HubPublishMessageSettings GetHubPublishMessageSettings(this PublishMessageModel model)
        {
            var stereotype = model.GetStereotype("Hub Publish Message Settings");
            return stereotype != null ? new HubPublishMessageSettings(stereotype) : null;
        }


        public static bool HasHubPublishMessageSettings(this PublishMessageModel model)
        {
            return model.HasStereotype("Hub Publish Message Settings");
        }

        public static bool TryGetHubPublishMessageSettings(this PublishMessageModel model, out HubPublishMessageSettings stereotype)
        {
            if (!HasHubPublishMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HubPublishMessageSettings(model.GetStereotype("Hub Publish Message Settings"));
            return true;
        }

        public class HubPublishMessageSettings
        {
            private IStereotype _stereotype;

            public HubPublishMessageSettings(IStereotype stereotype)
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
            }

            public enum TargetClientsOptionsEnum
            {
                All,
                User,
                Users,
                Group,
                Groups
            }
        }

    }
}