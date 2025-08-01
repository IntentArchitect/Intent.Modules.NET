using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CreateCustomerRich
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerRichCommandValidator : AbstractValidator<CreateCustomerRichCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerRichCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateCustomerRichAddressDto>()!);
        }
    }
}