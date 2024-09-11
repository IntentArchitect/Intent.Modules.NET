using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse200
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse200Validator : AbstractValidator<CustomResponse200>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse200Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}