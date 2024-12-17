using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Unversioned.Test
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestQueryValidator : AbstractValidator<TestQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public TestQueryValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull()
                .MustAsync(ValidateValueAsync)
                .WithMessage("Must be all lower case");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task<bool> ValidateValueAsync(TestQuery command, string value, CancellationToken cancellationToken)
        {
            // TODO: Implement ValidateValueAsync (TestQueryValidator) functionality
            throw new NotImplementedException("Your custom validation rules here...");
        }
    }
}