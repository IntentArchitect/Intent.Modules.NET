using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.UpdateCustomerAnemic
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerAnemicCommandValidator : AbstractValidator<UpdateCustomerAnemicCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateCustomerAnemicCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateCustomerAnemicAddressDto>()!);
        }
    }
}