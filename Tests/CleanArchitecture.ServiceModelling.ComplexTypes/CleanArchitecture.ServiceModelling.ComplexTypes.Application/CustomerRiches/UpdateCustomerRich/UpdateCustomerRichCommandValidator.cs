using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.UpdateCustomerRich
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerRichCommandValidator : AbstractValidator<UpdateCustomerRichCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerRichCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateCustomerRichAddressDto>()!);
        }
    }
}