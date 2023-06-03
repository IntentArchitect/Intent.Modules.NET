using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.MongoDb.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static Collection GetCollection(this FolderModel model)
        {
            var stereotype = model.GetStereotype("Collection");
            return stereotype != null ? new Collection(stereotype) : null;
        }


        public static bool HasCollection(this FolderModel model)
        {
            return model.HasStereotype("Collection");
        }

        public static bool TryGetCollection(this FolderModel model, out Collection stereotype)
        {
            if (!HasCollection(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Collection(model.GetStereotype("Collection"));
            return true;
        }

        public class Collection
        {
            private IStereotype _stereotype;

            public Collection(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

        }

    }
}