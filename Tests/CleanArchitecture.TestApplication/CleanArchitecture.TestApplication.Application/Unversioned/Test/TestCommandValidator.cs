using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.Unversioned.Test
{
    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
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

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        private async Task<bool> ValidateValueAsync(TestCommand command, string value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your custom validation rules here...");
            return true;
        }
    }
}