using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAggregateWithUniqueConstraintIndexStereotypeCommandValidator : AbstractValidator<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateWithUniqueConstraintIndexStereotypeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}