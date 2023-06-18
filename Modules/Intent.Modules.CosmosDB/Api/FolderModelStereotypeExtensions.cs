using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static Container GetContainer(this FolderModel model)
        {
            var stereotype = model.GetStereotype("Container");
            return stereotype != null ? new Container(stereotype) : null;
        }


        public static bool HasContainer(this FolderModel model)
        {
            return model.HasStereotype("Container");
        }

        public static bool TryGetContainer(this FolderModel model, out Container stereotype)
        {
            if (!HasContainer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Container(model.GetStereotype("Container"));
            return true;
        }

        public class Container
        {
            private IStereotype _stereotype;

            public Container(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string PartitionKey()
            {
                return _stereotype.GetProperty<string>("Partition Key");
            }

        }

    }
}