using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Blazor.Components.MudBlazor.Api
{
    public static class ImageModelStereotypeExtensions
    {
        public static Appearance GetAppearance(this ImageModel model)
        {
            var stereotype = model.GetStereotype(Appearance.DefinitionId);
            return stereotype != null ? new Appearance(stereotype) : null;
        }


        public static bool HasAppearance(this ImageModel model)
        {
            return model.HasStereotype(Appearance.DefinitionId);
        }

        public static bool TryGetAppearance(this ImageModel model, out Appearance stereotype)
        {
            if (!HasAppearance(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Appearance(model.GetStereotype(Appearance.DefinitionId));
            return true;
        }

        public class Appearance
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "54eaa54d-6bed-4de6-8ee3-4c90ecd76a30";

            public Appearance(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? Elevation()
            {
                return _stereotype.GetProperty<int?>("Elevation");
            }

            public bool Fluid()
            {
                return _stereotype.GetProperty<bool>("Fluid");
            }

            public int? Width()
            {
                return _stereotype.GetProperty<int?>("Width");
            }

            public int? Height()
            {
                return _stereotype.GetProperty<int?>("Height");
            }

            public string Class()
            {
                return _stereotype.GetProperty<string>("Class");
            }

        }

    }
}