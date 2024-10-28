using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootCompositeManyBCommandValidator : AbstractValidator<UpdateAggregateRootCompositeManyBCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCompositeManyBCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();
        }
    }
}