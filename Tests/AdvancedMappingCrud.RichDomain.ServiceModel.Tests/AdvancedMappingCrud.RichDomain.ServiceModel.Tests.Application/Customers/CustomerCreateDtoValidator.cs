using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerCreateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.User)
                .NotNull()
                .SetValidator(provider.GetValidator<CustomerCreateCustomerUserDto>()!);

            RuleFor(v => v.Login)
                .NotNull()
                .MaximumLength(50);
        }
    }
}