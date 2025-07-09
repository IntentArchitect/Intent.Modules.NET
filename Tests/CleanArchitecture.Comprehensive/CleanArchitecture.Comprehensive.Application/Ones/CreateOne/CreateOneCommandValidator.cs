using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones.CreateOne
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneCommandValidator : AbstractValidator<CreateOneCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.OneId)
                .MustAsync(ValidateOneIdAsync)
                .WithMessage("A must custom message");

            RuleFor(v => v.Twos)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOneCommandTwosDto>()!));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        private async Task<bool> ValidateOneIdAsync(CreateOneCommand command, int value, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}