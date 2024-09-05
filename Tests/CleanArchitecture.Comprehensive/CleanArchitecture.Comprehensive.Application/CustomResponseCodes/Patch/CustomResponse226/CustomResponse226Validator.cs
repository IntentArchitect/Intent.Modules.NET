using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse226
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse226Validator : AbstractValidator<CustomResponse226>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse226Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}