using FluentValidation;
using FluentValidationTest.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.RegisterCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RegisterCustomerCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CustomerRegistration)
                .NotNull()
                .SetValidator(provider.GetValidator<CustomerRegistrationDto>()!);
        }
    }
}