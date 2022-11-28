using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.UpdateAggregateRootCompositeSingleA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeSingleACommandValidator : AbstractValidator<UpdateAggregateRootCompositeSingleACommand>
    {
        [IntentManaged(Mode.Fully)]
        public UpdateAggregateRootCompositeSingleACommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            

        }
    }
}