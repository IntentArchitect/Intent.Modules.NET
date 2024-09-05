using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse207
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse207Validator : AbstractValidator<CustomResponse207>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse207Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}