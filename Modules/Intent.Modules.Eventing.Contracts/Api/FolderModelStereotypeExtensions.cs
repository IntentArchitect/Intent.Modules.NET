using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.Contracts.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static MessageBus GetMessageBus(this FolderModel model)
        {
            var stereotype = model.GetStereotype(MessageBus.DefinitionId);
            return stereotype != null ? new MessageBus(stereotype) : null;
        }


        public static bool HasMessageBus(this FolderModel model)
        {
            return model.HasStereotype(MessageBus.DefinitionId);
        }

        public static bool TryGetMessageBus(this FolderModel model, out MessageBus stereotype)
        {
            if (!HasMessageBus(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MessageBus(model.GetStereotype(MessageBus.DefinitionId));
            return true;
        }

        public class MessageBus
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "2c0b73e9-b37a-4f6a-b08e-69eda4e2fdcf";

            public MessageBus(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement[] Providers()
            {
                return _stereotype.GetProperty<IElement[]>("Providers") ?? new IElement[0];
            }

        }

    }
}