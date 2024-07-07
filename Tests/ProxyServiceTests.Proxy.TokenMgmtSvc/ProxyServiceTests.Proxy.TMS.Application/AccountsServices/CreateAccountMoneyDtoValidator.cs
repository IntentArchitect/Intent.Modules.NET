using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.AccountsServices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAccountMoneyDtoValidator : AbstractValidator<CreateAccountMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAccountMoneyDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Currency)
                .NotNull()
                .IsInEnum();
        }
    }
}