using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponseDefault
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponseDefaultValidator : AbstractValidator<CustomResponseDefault>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponseDefaultValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}