using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static MassTransitMessage GetMassTransitMessage(this MessageModel model)
        {
            var stereotype = model.GetStereotype(MassTransitMessage.DefinitionId);
            return stereotype != null ? new MassTransitMessage(stereotype) : null;
        }


        public static bool HasMassTransitMessage(this MessageModel model)
        {
            return model.HasStereotype(MassTransitMessage.DefinitionId);
        }

        public static bool TryGetMassTransitMessage(this MessageModel model, out MassTransitMessage stereotype)
        {
            if (!HasMassTransitMessage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitMessage(model.GetStereotype(MassTransitMessage.DefinitionId));
            return true;
        }
        public static MessageTopologySettings GetMessageTopologySettings(this MessageModel model)
        {
            var stereotype = model.GetStereotype(MessageTopologySettings.DefinitionId);
            return stereotype != null ? new MessageTopologySettings(stereotype) : null;
        }


        public static bool HasMessageTopologySettings(this MessageModel model)
        {
            return model.HasStereotype(MessageTopologySettings.DefinitionId);
        }

        public static bool TryGetMessageTopologySettings(this MessageModel model, out MessageTopologySettings stereotype)
        {
            if (!HasMessageTopologySettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageTopologySettings(model.GetStereotype(MessageTopologySettings.DefinitionId));
            return true;
        }

        public class MassTransitMessage
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "19664e24-b935-4822-ac61-26d47488be42";

            public MassTransitMessage(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class MessageTopologySettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fc095295-eb25-470a-9ee5-19129919db2b";

            public MessageTopologySettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string EntityName()
            {
                return _stereotype.GetProperty<string>("Entity Name");
            }

        }

    }
}