using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse203
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse203Validator : AbstractValidator<CustomResponse203>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse203Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}