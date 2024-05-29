using System;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateTransaction
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTransactionCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Current)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateMoneyDto>()!);

            RuleFor(v => v.Description)
                .NotNull();

            RuleFor(v => v.Account)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateAccountDto>()!);
        }
    }
}