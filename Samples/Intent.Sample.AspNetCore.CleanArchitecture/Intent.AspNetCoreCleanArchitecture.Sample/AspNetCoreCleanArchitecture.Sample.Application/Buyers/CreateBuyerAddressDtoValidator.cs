using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBuyerAddressDtoValidator : AbstractValidator<CreateBuyerAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBuyerAddressDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull()
                .MaximumLength(200);

            RuleFor(v => v.Line2)
                .NotNull()
                .MaximumLength(200);

            RuleFor(v => v.City)
                .NotNull()
                .MaximumLength(200);

            RuleFor(v => v.PostalCode)
                .NotNull()
                .MaximumLength(20);
        }
    }
}