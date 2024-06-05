using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.UpdateAccount
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAccountCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Number)
                .NotNull();

            RuleFor(v => v.Amount)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateAccountMoneyDto>()!);
        }
    }
}