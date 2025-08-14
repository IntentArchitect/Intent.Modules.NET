using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.MongoDb.MongoFramework.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static Collection GetCollection(this ClassModel model)
        {
            var stereotype = model.GetStereotype(Collection.DefinitionId);
            return stereotype != null ? new Collection(stereotype) : null;
        }


        public static bool HasCollection(this ClassModel model)
        {
            return model.HasStereotype(Collection.DefinitionId);
        }

        public static bool TryGetCollection(this ClassModel model, out Collection stereotype)
        {
            if (!HasCollection(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Collection(model.GetStereotype(Collection.DefinitionId));
            return true;
        }

        public class Collection
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "677d2bdb-f9ff-43c4-8c91-bb4446c1afbc";

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