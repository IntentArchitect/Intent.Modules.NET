using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerUserContactDetailsVODtoValidator : AbstractValidator<CreateCustomerUserContactDetailsVODto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerUserContactDetailsVODtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Cell)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}