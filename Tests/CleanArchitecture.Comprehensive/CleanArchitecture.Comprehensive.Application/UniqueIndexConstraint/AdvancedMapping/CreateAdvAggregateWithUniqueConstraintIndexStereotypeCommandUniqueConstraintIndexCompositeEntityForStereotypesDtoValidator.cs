using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDtoValidator : AbstractValidator<CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull();
        }
    }
}