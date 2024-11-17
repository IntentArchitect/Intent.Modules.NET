using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.CreateBuyer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBuyerCommandValidator : AbstractValidator<CreateBuyerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBuyerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}