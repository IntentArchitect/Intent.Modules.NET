using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerCreateCustomerUserDtoValidator : AbstractValidator<CustomerCreateCustomerUserDto>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerCreateCustomerUserDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.ContactDetailsVO)
                .NotNull()
                .SetValidator(provider.GetValidator<CustomerCreateCustomerUserContactDetailsVODto>()!);

            RuleFor(v => v.Addresses)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CustomerCreateCustomerUserAddressesDto>()!));
        }
    }
}