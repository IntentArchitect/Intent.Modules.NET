using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();

            RuleFor(v => v.Address)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateCustomerCommandAddressDto>()!));
        }
    }
}