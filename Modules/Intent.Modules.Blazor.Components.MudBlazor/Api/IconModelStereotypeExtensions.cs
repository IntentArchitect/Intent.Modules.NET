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

            public class IconValueOptions
            {
                public readonly string Value;

                public IconValueOptions(string value)
                {
                    Value = value;
                }

                public IconValueOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return IconValueOptionsEnum.Default;
                        case "Primary":
                            return IconValueOptionsEnum.Primary;
                        case "Secondary":
                            return IconValueOptionsEnum.Secondary;
                        case "Tertiary":
                            return IconValueOptionsEnum.Tertiary;
                        case "Info":
                            return IconValueOptionsEnum.Info;
                        case "Success":
                            return IconValueOptionsEnum.Success;
                        case "Warning":
                            return IconValueOptionsEnum.Warning;
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

            public enum IconValueOptionsEnum
            {
                Default,
                Primary,
                Secondary,
                Tertiary,
                Info,
                Success,
                Warning
            }
            public class IconColorOptions
            {
                public readonly string Value;

                public IconColorOptions(string value)
                {
                    Value = value;
                }

                public IconColorOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return IconColorOptionsEnum.Default;
                        case "Primary":
                            return IconColorOptionsEnum.Primary;
                        case "Secondary":
                            return IconColorOptionsEnum.Secondary;
                        case "Tertiary":
                            return IconColorOptionsEnum.Tertiary;
                        case "Info":
                            return IconColorOptionsEnum.Info;
                        case "Success":
                            return IconColorOptionsEnum.Success;
                        case "Warning":
                            return IconColorOptionsEnum.Warning;
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

            public enum IconColorOptionsEnum
            {
                Default,
                Primary,
                Secondary,
                Tertiary,
                Info,
                Success,
                Warning
            }
            public class VariantOptions
            {
                public readonly string Value;

                public VariantOptions(string value)
                {
                    Value = value;
                }

                public VariantOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return VariantOptionsEnum.Default;
                        case "Primary":
                            return VariantOptionsEnum.Primary;
                        case "Secondary":
                            return VariantOptionsEnum.Secondary;
                        case "Tertiary":
                            return VariantOptionsEnum.Tertiary;
                        case "Info":
                            return VariantOptionsEnum.Info;
                        case "Success":
                            return VariantOptionsEnum.Success;
                        case "Warning":
                            return VariantOptionsEnum.Warning;
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

            public enum VariantOptionsEnum
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