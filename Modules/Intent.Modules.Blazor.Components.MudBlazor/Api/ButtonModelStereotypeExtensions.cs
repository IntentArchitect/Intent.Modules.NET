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

            public IElement Color()
            {
                return _stereotype.GetProperty<IElement>("Color");
            }

            public class ColorOptions
            {
                public readonly string Value;

                public ColorOptions(string value)
                {
                    Value = value;
                }

                public ColorOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return ColorOptionsEnum.Default;
                        case "Primary":
                            return ColorOptionsEnum.Primary;
                        case "Secondary":
                            return ColorOptionsEnum.Secondary;
                        case "Tertiary":
                            return ColorOptionsEnum.Tertiary;
                        case "Info":
                            return ColorOptionsEnum.Info;
                        case "Success":
                            return ColorOptionsEnum.Success;
                        case "Warning":
                            return ColorOptionsEnum.Warning;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsPrimary()
                {
                    return Value == "Primary";
                }
                public bool IsSecondary()
                {
                    return Value == "Secondary";
                }
                public bool IsTertiary()
                {
                    return Value == "Tertiary";
                }
                public bool IsInfo()
                {
                    return Value == "Info";
                }
                public bool IsSuccess()
                {
                    return Value == "Success";
                }
                public bool IsWarning()
                {
                    return Value == "Warning";
                }
            }

            public enum ColorOptionsEnum
            {
                Default,
                Primary,
                Secondary,
                Tertiary,
                Info,
                Success,
                Warning
            }

        }

    }
}