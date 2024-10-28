using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.DeleteAggregateRoot
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAggregateRootCommandValidator : AbstractValidator<DeleteAggregateRootCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateRootCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}