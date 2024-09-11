using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Application.FluentValidation.Api
{
    public static class DTOFieldModelStereotypeExtensions
    {
        public static Validations GetValidations(this DTOFieldModel model)
        {
            var stereotype = model.GetStereotype("4b54a612-2664-4493-a1f7-dc0623aa03da");
            return stereotype != null ? new Validations(stereotype) : null;
        }


        public static bool HasValidations(this DTOFieldModel model)
        {
            return model.HasStereotype("4b54a612-2664-4493-a1f7-dc0623aa03da");
        }

        public static bool TryGetValidations(this DTOFieldModel model, out Validations stereotype)
        {
            if (!HasValidations(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Validations(model.GetStereotype("4b54a612-2664-4493-a1f7-dc0623aa03da"));
            return true;
        }


        public class Validations
        {
            private IStereotype _stereotype;

            public Validations(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool NotEmpty()
            {
                return _stereotype.GetProperty<bool>("Not Empty");
            }

            public string Equal()
            {
                return _stereotype.GetProperty<string>("Equal");
            }

            public string NotEqual()
            {
                return _stereotype.GetProperty<string>("Not Equal");
            }

            public int? MinLength()
            {
                return _stereotype.GetProperty<int?>("Min Length");
            }

            public int? MaxLength()
            {
                return _stereotype.GetProperty<int?>("Max Length");
            }

            public string Min()
            {
                return _stereotype.GetProperty<string>("Min");
            }

            public string Max()
            {
                return _stereotype.GetProperty<string>("Max");
            }

            public string RegularExpression()
            {
                return _stereotype.GetProperty<string>("Regular Expression");
            }

            public string Predicate()
            {
                return _stereotype.GetProperty<string>("Predicate");
            }

            public string PredicateMessage()
            {
                return _stereotype.GetProperty<string>("Predicate Message");
            }

            public bool EmailAddress()
            {
                return _stereotype.GetProperty<bool>("Email Address");
            }

            public bool HasCustomValidation()
            {
                return _stereotype.GetProperty<bool>("Has Custom Validation");
            }

            public bool Custom()
            {
                return _stereotype.GetProperty<bool>("Custom");
            }

            public bool Must()
            {
                return _stereotype.GetProperty<bool>("Must");
            }

            public CascadeModeOptions CascadeMode()
            {
                return new CascadeModeOptions(_stereotype.GetProperty<string>("CascadeMode"));
            }

            public class CascadeModeOptions
            {
                public readonly string Value;

                public CascadeModeOptions(string value)
                {
                    Value = value;
                }

                public CascadeModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Continue":
                            return CascadeModeOptionsEnum.Continue;
                        case "Stop":
                            return CascadeModeOptionsEnum.Stop;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsContinue()
                {
                    return Value == "Continue";
                }
                public bool IsStop()
                {
                    return Value == "Stop";
                }
            }

            public enum CascadeModeOptionsEnum
            {
                Continue,
                Stop
            }

        }

    }
}