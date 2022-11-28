using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.CreateAggregateRootCompositeSingleA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootCompositeSingleACommandValidator : AbstractValidator<CreateAggregateRootCompositeSingleACommand>
    {
        [IntentManaged(Mode.Fully)]
        public CreateAggregateRootCompositeSingleACommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            

        }
    }
}