using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Entities.SoftDelete.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static SoftDeleteEntity GetSoftDeleteEntity(this ClassModel model)
        {
            var stereotype = model.GetStereotype(SoftDeleteEntity.DefinitionId);
            return stereotype != null ? new SoftDeleteEntity(stereotype) : null;
        }


        public static bool HasSoftDeleteEntity(this ClassModel model)
        {
            return model.HasStereotype(SoftDeleteEntity.DefinitionId);
        }

        public static bool TryGetSoftDeleteEntity(this ClassModel model, out SoftDeleteEntity stereotype)
        {
            if (!HasSoftDeleteEntity(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SoftDeleteEntity(model.GetStereotype(SoftDeleteEntity.DefinitionId));
            return true;
        }

        public class SoftDeleteEntity
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "65860af3-8805-4a63-9fb9-3884b80f4380";

            public SoftDeleteEntity(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}