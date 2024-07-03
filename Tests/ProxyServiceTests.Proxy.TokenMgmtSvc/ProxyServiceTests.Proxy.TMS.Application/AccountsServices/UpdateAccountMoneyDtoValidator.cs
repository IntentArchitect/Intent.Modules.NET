using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.AccountsServices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAccountMoneyDtoValidator : AbstractValidator<UpdateAccountMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAccountMoneyDtoValidator()
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