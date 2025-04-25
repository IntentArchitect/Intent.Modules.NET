using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.CreateAdvAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAdvAggregateWithUniqueConstraintIndexElementCommandValidator : AbstractValidator<CreateAdvAggregateWithUniqueConstraintIndexElementCommand>
    {
        [IntentManaged(Mode.Fully)]
        public CreateAdvAggregateWithUniqueConstraintIndexElementCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
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

            RuleFor(v => v.UniqueConstraintIndexCompositeEntityForElements)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateAdvAggregateWithUniqueConstraintIndexElementCommandUniqueConstraintIndexCompositeEntityForElementsDto>()!));
        }
    }
}