using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Blazor.Components.MudBlazor.Api
{
    public static class DataGridModelStereotypeExtensions
    {
        public static Appearance GetAppearance(this DataGridModel model)
        {
            var stereotype = model.GetStereotype(Appearance.DefinitionId);
            return stereotype != null ? new Appearance(stereotype) : null;
        }


        public static bool HasAppearance(this DataGridModel model)
        {
            return model.HasStereotype(Appearance.DefinitionId);
        }

        public static bool TryGetAppearance(this DataGridModel model, out Appearance stereotype)
        {
            if (!HasAppearance(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Appearance(model.GetStereotype(Appearance.DefinitionId));
            return true;
        }
        public static Interaction GetInteraction(this DataGridModel model)
        {
            var stereotype = model.GetStereotype(Interaction.DefinitionId);
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this DataGridModel model)
        {
            return model.HasStereotype(Interaction.DefinitionId);
        }

        public static bool TryGetInteraction(this DataGridModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype(Interaction.DefinitionId));
            return true;
        }

        public class Appearance
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d523028b-4077-441e-a70f-50af42c9e91f";

            public Appearance(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public DenseOptions Dense()
            {
                return new DenseOptions(_stereotype.GetProperty<string>("Dense"));
            }

            public int? Elevation()
            {
                return _stereotype.GetProperty<int?>("Elevation");
            }

            public HoverOptions Hover()
            {
                return new HoverOptions(_stereotype.GetProperty<string>("Hover"));
            }

            public StripedOptions Striped()
            {
                return new StripedOptions(_stereotype.GetProperty<string>("Striped"));
            }

            public class DenseOptions
            {
                public readonly string Value;

                public DenseOptions(string value)
                {
                    Value = value;
                }

                public DenseOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "true":
                            return DenseOptionsEnum.True;
                        case "false":
                            return DenseOptionsEnum.False;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsTrue()
                {
                    return Value == "true";
                }
                public bool IsFalse()
                {
                    return Value == "false";
                }
            }

            public enum DenseOptionsEnum
            {
                True,
                False
            }
            public class HoverOptions
            {
                public readonly string Value;

                public HoverOptions(string value)
                {
                    Value = value;
                }

                public HoverOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "true":
                            return HoverOptionsEnum.True;
                        case "false":
                            return HoverOptionsEnum.False;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsTrue()
                {
                    return Value == "true";
                }
                public bool IsFalse()
                {
                    return Value == "false";
                }
            }

            public enum HoverOptionsEnum
            {
                True,
                False
            }
            public class StripedOptions
            {
                public readonly string Value;

                public StripedOptions(string value)
                {
                    Value = value;
                }

                public StripedOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "true":
                            return StripedOptionsEnum.True;
                        case "false":
                            return StripedOptionsEnum.False;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsTrue()
                {
                    return Value == "true";
                }
                public bool IsFalse()
                {
                    return Value == "false";
                }
            }

            public enum StripedOptionsEnum
            {
                True,
                False
            }
        }

        public class Interaction
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "0f3fc9f6-d11b-4e78-b28d-3c818e32d510";

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OnRowClick()
            {
                return _stereotype.GetProperty<string>("On Row Click");
            }

        }

    }
}