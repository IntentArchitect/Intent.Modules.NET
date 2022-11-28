using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.DeleteAggregateRootCompositeSingleA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootCompositeSingleACommandValidator : AbstractValidator<DeleteAggregateRootCompositeSingleACommand>
    {
        [IntentManaged(Mode.Fully)]
        public DeleteAggregateRootCompositeSingleACommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}