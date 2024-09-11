using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponse202
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse202Validator : AbstractValidator<CustomResponse202>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse202Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}