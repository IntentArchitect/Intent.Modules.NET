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
            var stereotype = model.GetStereotype("Validations");
            return stereotype != null ? new Validations(stereotype) : null;
        }


        public static bool HasValidations(this DTOFieldModel model)
        {
            return model.HasStereotype("Validations");
        }

        public static bool TryGetValidations(this DTOFieldModel model, out Validations stereotype)
        {
            if (!HasValidations(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Validations(model.GetStereotype("Validations"));
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

            public string Predicate()
            {
                return _stereotype.GetProperty<string>("Predicate");
            }

            public string PredicateMessage()
            {
                return _stereotype.GetProperty<string>("Predicate Message");
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

        }

    }
}