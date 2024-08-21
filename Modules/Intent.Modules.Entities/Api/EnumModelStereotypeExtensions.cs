using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Entities.Api
{
    public static class EnumModelStereotypeExtensions
    {
        public static Flags GetFlags(this EnumModel model)
        {
            var stereotype = model.GetStereotype("3988663b-69c9-4ab1-b471-ecd3bc01dec6");
            return stereotype != null ? new Flags(stereotype) : null;
        }


        public static bool HasFlags(this EnumModel model)
        {
            return model.HasStereotype("3988663b-69c9-4ab1-b471-ecd3bc01dec6");
        }

        public static bool TryGetFlags(this EnumModel model, out Flags stereotype)
        {
            if (!HasFlags(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Flags(model.GetStereotype("3988663b-69c9-4ab1-b471-ecd3bc01dec6"));
            return true;
        }

        public class Flags
        {
            private IStereotype _stereotype;

            public Flags(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}