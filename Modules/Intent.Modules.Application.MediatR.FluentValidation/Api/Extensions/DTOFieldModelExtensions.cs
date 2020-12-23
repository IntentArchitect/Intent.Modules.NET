using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Application.MediatR.FluentValidation.Api
{
    public static class DTOFieldModelExtensions
    {
        public static StringValidation GetStringValidation(this DTOFieldModel model)
        {
            var stereotype = model.GetStereotype("String Validation");
            return stereotype != null ? new StringValidation(stereotype) : null;
        }

        public static bool HasStringValidation(this DTOFieldModel model)
        {
            return model.HasStereotype("String Validation");
        }


        public class StringValidation
        {
            private IStereotype _stereotype;

            public StringValidation(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool NotEmpty()
            {
                return _stereotype.GetProperty<bool>("Not Empty");
            }

            public int? MaxLength()
            {
                return _stereotype.GetProperty<int?>("Max Length");
            }

            public bool HasCustomValidation()
            {
                return _stereotype.GetProperty<bool>("Has Custom Validation");
            }

        }

    }
}