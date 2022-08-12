using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Api
{
    public static class DomainPackageModelStereotypeExtensions
    {
        public static CosmosDBContainerSettings GetCosmosDBContainerSettings(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype("Cosmos DB Container Settings");
            return stereotype != null ? new CosmosDBContainerSettings(stereotype) : null;
        }


        public static bool HasCosmosDBContainerSettings(this DomainPackageModel model)
        {
            return model.HasStereotype("Cosmos DB Container Settings");
        }

        public static bool TryGetCosmosDBContainerSettings(this DomainPackageModel model, out CosmosDBContainerSettings stereotype)
        {
            if (!HasCosmosDBContainerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CosmosDBContainerSettings(model.GetStereotype("Cosmos DB Container Settings"));
            return true;
        }

        public class CosmosDBContainerSettings
        {
            private IStereotype _stereotype;

            public CosmosDBContainerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ContainerName()
            {
                return _stereotype.GetProperty<string>("Container Name");
            }

            public string PartitionKey()
            {
                return _stereotype.GetProperty<string>("Partition Key");
            }

        }

    }
}