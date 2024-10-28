using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.UpdateAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommandValidator : AbstractValidator<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateWithUniqueConstraintIndexStereotypeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SingleUniqueField)
                .NotNull()
                .MaximumLength(256);

            RuleFor(v => v.CompUniqueFieldA)
                .NotNull()
                .MaximumLength(256);

            RuleFor(v => v.CompUniqueFieldB)
                .NotNull()
                .MaximumLength(256);
        }
    }
}