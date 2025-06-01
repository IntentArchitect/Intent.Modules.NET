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
    public static class IconModelStereotypeExtensions
    {
        public static Icon GetIcon(this IconModel model)
        {
            var stereotype = model.GetStereotype(Icon.DefinitionId);
            return stereotype != null ? new Icon(stereotype) : null;
        }


        public static bool HasIcon(this IconModel model)
        {
            return model.HasStereotype(Icon.DefinitionId);
        }

        public static bool TryGetIcon(this IconModel model, out Icon stereotype)
        {
            if (!HasIcon(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Icon(model.GetStereotype(Icon.DefinitionId));
            return true;
        }

        public class Icon
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8e1b7033-fd27-495a-a2a7-36b5168f04f5";

            public Icon(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement IconValue()
            {
                return _stereotype.GetProperty<IElement>("Icon Value");
            }

            public IElement IconColor()
            {
                return _stereotype.GetProperty<IElement>("Icon Color");
            }

            public IElement Variant()
            {
                return _stereotype.GetProperty<IElement>("Variant");
            }
        }

    }
}