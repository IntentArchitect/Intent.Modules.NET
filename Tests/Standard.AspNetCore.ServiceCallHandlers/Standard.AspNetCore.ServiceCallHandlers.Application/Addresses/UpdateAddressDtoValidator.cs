using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Addresses
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAddressDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.Line2)
                .NotNull();
        }
    }
}