using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse206
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse206Validator : AbstractValidator<CustomResponse206>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse206Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}