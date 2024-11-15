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
    public static class ClassModelStereotypeExtensions
    {
        public static CosmosDBContainerSettings GetCosmosDBContainerSettings(this ClassModel model)
        {
            var stereotype = model.GetStereotype(CosmosDBContainerSettings.DefinitionId);
            return stereotype != null ? new CosmosDBContainerSettings(stereotype) : null;
        }


        public static bool HasCosmosDBContainerSettings(this ClassModel model)
        {
            return model.HasStereotype(CosmosDBContainerSettings.DefinitionId);
        }

        public static bool TryGetCosmosDBContainerSettings(this ClassModel model, out CosmosDBContainerSettings stereotype)
        {
            if (!HasCosmosDBContainerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CosmosDBContainerSettings(model.GetStereotype(CosmosDBContainerSettings.DefinitionId));
            return true;
        }

        public class CosmosDBContainerSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b4995259-b47b-405a-a332-fd3dc69cd3a5";

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