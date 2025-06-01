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
    public static class TextInputModelStereotypeExtensions
    {
        public static Appearance GetAppearance(this TextInputModel model)
        {
            var stereotype = model.GetStereotype(Appearance.DefinitionId);
            return stereotype != null ? new Appearance(stereotype) : null;
        }


        public static bool HasAppearance(this TextInputModel model)
        {
            return model.HasStereotype(Appearance.DefinitionId);
        }

        public static bool TryGetAppearance(this TextInputModel model, out Appearance stereotype)
        {
            if (!HasAppearance(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Appearance(model.GetStereotype(Appearance.DefinitionId));
            return true;
        }

        public static Behaviours GetBehaviours(this TextInputModel model)
        {
            var stereotype = model.GetStereotype(Behaviours.DefinitionId);
            return stereotype != null ? new Behaviours(stereotype) : null;
        }


        public static bool HasBehaviours(this TextInputModel model)
        {
            return model.HasStereotype(Behaviours.DefinitionId);
        }

        public static bool TryGetBehaviours(this TextInputModel model, out Behaviours stereotype)
        {
            if (!HasBehaviours(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Behaviours(model.GetStereotype(Behaviours.DefinitionId));
            return true;
        }

        public static Icon GetIcon(this TextInputModel model)
        {
            var stereotype = model.GetStereotype(Icon.DefinitionId);
            return stereotype != null ? new Icon(stereotype) : null;
        }


        public static bool HasIcon(this TextInputModel model)
        {
            return model.HasStereotype(Icon.DefinitionId);
        }

        public static bool TryGetIcon(this TextInputModel model, out Icon stereotype)
        {
            if (!HasIcon(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Icon(model.GetStereotype(Icon.DefinitionId));
            return true;
        }

        public class Appearance
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5b19f111-14dc-40e0-a71b-9758d1b88026";

            public Appearance(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement Variant()
            {
                return _stereotype.GetProperty<IElement>("Variant");
            }

            public string Placeholder()
            {
                return _stereotype.GetProperty<string>("Placeholder");
            }

            public string Class()
            {
                return _stereotype.GetProperty<string>("Class");
            }
        }

        public class Behaviours
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b3d9be90-aaa8-4bf7-b8d0-95c73a6ee68a";

            public Behaviours(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool Immediate()
            {
                return _stereotype.GetProperty<bool>("Immediate");
            }

            public int? DebounceInterval()
            {
                return _stereotype.GetProperty<int?>("Debounce Interval");
            }

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