using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.SoftDelete.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static SoftDeleteEntity GetSoftDeleteEntity(this ClassModel model)
        {
            var stereotype = model.GetStereotype("Soft Delete Entity");
            return stereotype != null ? new SoftDeleteEntity(stereotype) : null;
        }


        public static bool HasSoftDeleteEntity(this ClassModel model)
        {
            return model.HasStereotype("Soft Delete Entity");
        }

        public static bool TryGetSoftDeleteEntity(this ClassModel model, out SoftDeleteEntity stereotype)
        {
            if (!HasSoftDeleteEntity(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SoftDeleteEntity(model.GetStereotype("Soft Delete Entity"));
            return true;
        }

        public class SoftDeleteEntity
        {
            private IStereotype _stereotype;

            public SoftDeleteEntity(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}