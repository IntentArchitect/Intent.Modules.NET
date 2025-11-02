using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureQueueStorage.Api
{
    public static class IntegrationCommandModelStereotypeExtensions
    {
        public static AzureQueueStorage GetAzureQueueStorage(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(AzureQueueStorage.DefinitionId);
            return stereotype != null ? new AzureQueueStorage(stereotype) : null;
        }


        public static bool HasAzureQueueStorage(this IntegrationCommandModel model)
        {
            return model.HasStereotype(AzureQueueStorage.DefinitionId);
        }

        public static bool TryGetAzureQueueStorage(this IntegrationCommandModel model, out AzureQueueStorage stereotype)
        {
            if (!HasAzureQueueStorage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureQueueStorage(model.GetStereotype(AzureQueueStorage.DefinitionId));
            return true;
        }

        public class AzureQueueStorage
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7b57f640-600d-4b91-98a7-2a304c715f27";

            public AzureQueueStorage(IStereotype stereotype)
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