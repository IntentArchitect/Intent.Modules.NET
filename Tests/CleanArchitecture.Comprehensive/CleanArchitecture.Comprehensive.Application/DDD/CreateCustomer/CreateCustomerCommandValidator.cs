using System;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Surname)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateCustomerAddressDto>()!);

            RuleFor(v => v.Email)
                .NotNull()
                .MaximumLength(100);
        }
    }
}