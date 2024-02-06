using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Redis.Om.Repositories.Api
{
    public static class AttributeModelStereotypeExtensions
    {
        public static FieldSetting GetFieldSetting(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("b92797e6-52ec-464e-80d3-2274cc97a1a1");
            return stereotype != null ? new FieldSetting(stereotype) : null;
        }


        public static bool HasFieldSetting(this AttributeModel model)
        {
            return model.HasStereotype("b92797e6-52ec-464e-80d3-2274cc97a1a1");
        }

        public static bool TryGetFieldSetting(this AttributeModel model, out FieldSetting stereotype)
        {
            if (!HasFieldSetting(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FieldSetting(model.GetStereotype("b92797e6-52ec-464e-80d3-2274cc97a1a1"));
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