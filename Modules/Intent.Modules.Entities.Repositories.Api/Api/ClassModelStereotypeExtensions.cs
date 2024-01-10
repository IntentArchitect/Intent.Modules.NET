using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Entities.Repositories.Api.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static Repository GetRepository(this ClassModel model)
        {
            var stereotype = model.GetStereotype("cdc3d50d-0edd-4ffd-996f-08cbdf39595c");
            return stereotype != null ? new Repository(stereotype) : null;
        }


        public static bool HasRepository(this ClassModel model)
        {
            return model.HasStereotype("cdc3d50d-0edd-4ffd-996f-08cbdf39595c");
        }

        public static bool TryGetRepository(this ClassModel model, out Repository stereotype)
        {
            if (!HasRepository(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Repository(model.GetStereotype("cdc3d50d-0edd-4ffd-996f-08cbdf39595c"));
            return true;
        }


        public class Repository
        {
            private IStereotype _stereotype;

            public Repository(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}