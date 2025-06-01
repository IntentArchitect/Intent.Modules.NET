using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureEventGrid.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Ignore)]
    internal static class MessageModelStereotypeExtensions
    {
        public static AzureEventGrid GetAzureEventGrid(this MessageModel model)
        {
            var stereotype = model.GetStereotype(AzureEventGrid.DefinitionId);
            return stereotype != null ? new AzureEventGrid(stereotype) : null;
        }


        public static bool HasAzureEventGrid(this MessageModel model)
        {
            return model.HasStereotype(AzureEventGrid.DefinitionId);
        }

        public static bool TryGetAzureEventGrid(this MessageModel model, out AzureEventGrid stereotype)
        {
            if (!HasAzureEventGrid(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureEventGrid(model.GetStereotype(AzureEventGrid.DefinitionId));
            return true;
        }

        public class AzureEventGrid
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "dca28d4b-c277-4fb3-afe0-17f35ea8b59b";

            public AzureEventGrid(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string TopicName()
            {
                return _stereotype.GetProperty<string>("Topic Name");
            }

        }

    }
}