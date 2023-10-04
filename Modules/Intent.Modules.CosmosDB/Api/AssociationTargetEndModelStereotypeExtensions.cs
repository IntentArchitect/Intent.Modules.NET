using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.CosmosDB.Api
{
    public static class AssociationTargetEndModelStereotypeExtensions
    {
        public static FieldSetting GetFieldSetting(this AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Field Setting");
            return stereotype != null ? new FieldSetting(stereotype) : null;
        }


        public static bool HasFieldSetting(this AssociationTargetEndModel model)
        {
            return model.HasStereotype("Field Setting");
        }

        public static bool TryGetFieldSetting(this AssociationTargetEndModel model, out FieldSetting stereotype)
        {
            if (!HasFieldSetting(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FieldSetting(model.GetStereotype("Field Setting"));
            return true;
        }

        public class FieldSetting
        {
            private IStereotype _stereotype;

            public FieldSetting(IStereotype stereotype)
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