using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.DDD.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateCustomerCommandValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Surname)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Email)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetRequiredService<IValidator<CreateCustomerAddressDto>>()!);
        }
    }
}