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
    public static class ButtonModelStereotypeExtensions
    {
        public static Appearance GetAppearance(this ButtonModel model)
        {
            var stereotype = model.GetStereotype(Appearance.DefinitionId);
            return stereotype != null ? new Appearance(stereotype) : null;
        }


        public static bool HasAppearance(this ButtonModel model)
        {
            return model.HasStereotype(Appearance.DefinitionId);
        }

        public static bool TryGetAppearance(this ButtonModel model, out Appearance stereotype)
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
            public const string DefinitionId = "b218f7cb-d150-401d-a17a-9e22fadf863f";

            public Appearance(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}