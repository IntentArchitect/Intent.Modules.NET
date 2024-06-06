using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Unversioned.Test
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull()
                .MustAsync(ValidateValueAsync);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task<bool> ValidateValueAsync(TestCommand command, string value, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}