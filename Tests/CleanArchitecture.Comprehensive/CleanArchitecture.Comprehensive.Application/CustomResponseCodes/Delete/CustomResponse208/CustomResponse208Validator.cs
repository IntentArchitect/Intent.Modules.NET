using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponse208
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse208Validator : AbstractValidator<CustomResponse208>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse208Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}