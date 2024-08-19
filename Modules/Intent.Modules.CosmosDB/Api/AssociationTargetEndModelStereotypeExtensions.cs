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
            var stereotype = model.GetStereotype("fb47f1e4-447b-4a67-947d-590fc24c20c1");
            return stereotype != null ? new FieldSetting(stereotype) : null;
        }


        public static bool HasFieldSetting(this AssociationTargetEndModel model)
        {
            return model.HasStereotype("fb47f1e4-447b-4a67-947d-590fc24c20c1");
        }

        public static bool TryGetFieldSetting(this AssociationTargetEndModel model, out FieldSetting stereotype)
        {
            if (!HasFieldSetting(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FieldSetting(model.GetStereotype("fb47f1e4-447b-4a67-947d-590fc24c20c1"));
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