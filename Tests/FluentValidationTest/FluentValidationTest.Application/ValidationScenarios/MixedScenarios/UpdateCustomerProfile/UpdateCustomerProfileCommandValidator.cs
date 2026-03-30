using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.UpdateCustomerProfile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerProfileCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DisplayName)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}