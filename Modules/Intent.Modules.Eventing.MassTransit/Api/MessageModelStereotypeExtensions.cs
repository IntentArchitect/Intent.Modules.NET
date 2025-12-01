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
        public static MassTransitMessageSettings GetMassTransitMessageSettings(this MessageModel model)
        {
            var stereotype = model.GetStereotype(MassTransitMessageSettings.DefinitionId);
            return stereotype != null ? new MassTransitMessageSettings(stereotype) : null;
        }


        public static bool HasMassTransitMessageSettings(this MessageModel model)
        {
            return model.HasStereotype(MassTransitMessageSettings.DefinitionId);
        }

        public static bool TryGetMassTransitMessageSettings(this MessageModel model, out MassTransitMessageSettings stereotype)
        {
            if (!HasMassTransitMessageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitMessageSettings(model.GetStereotype(MassTransitMessageSettings.DefinitionId));
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

        public class MassTransitMessageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fbe53252-9913-453c-b734-73b4e2dfdb46";

            public MassTransitMessageSettings(IStereotype stereotype)
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