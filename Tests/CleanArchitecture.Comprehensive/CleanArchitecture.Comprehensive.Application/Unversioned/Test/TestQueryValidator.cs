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
        [IntentManaged(Mode.Merge)]
        public TestQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull()
                .MustAsync(ValidateValueAsync)
                .WithMessage("Must be all lower case");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        private async Task<bool> ValidateValueAsync(TestQuery command, string value, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}