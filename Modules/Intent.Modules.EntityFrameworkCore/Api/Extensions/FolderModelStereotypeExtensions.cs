using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static CosmosDBContainerSettings GetCosmosDBContainerSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype("b4995259-b47b-405a-a332-fd3dc69cd3a5");
            return stereotype != null ? new CosmosDBContainerSettings(stereotype) : null;
        }


        public static bool HasCosmosDBContainerSettings(this FolderModel model)
        {
            return model.HasStereotype("b4995259-b47b-405a-a332-fd3dc69cd3a5");
        }

        public static bool TryGetCosmosDBContainerSettings(this FolderModel model, out CosmosDBContainerSettings stereotype)
        {
            if (!HasCosmosDBContainerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CosmosDBContainerSettings(model.GetStereotype("b4995259-b47b-405a-a332-fd3dc69cd3a5"));
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