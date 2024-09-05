using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse204
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse204Validator : AbstractValidator<CustomResponse204>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse204Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}