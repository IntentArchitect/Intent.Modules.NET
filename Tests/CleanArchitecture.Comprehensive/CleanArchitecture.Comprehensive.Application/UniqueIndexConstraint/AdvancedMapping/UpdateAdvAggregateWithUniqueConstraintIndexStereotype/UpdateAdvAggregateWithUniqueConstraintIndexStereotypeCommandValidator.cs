using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.UpdateAdvAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandValidator : AbstractValidator<UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandValidator(IValidatorProvider provider, IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository)
        {
            ConfigureValidationRules(provider);
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.SingleUniqueField)
                .NotNull()
                .MaximumLength(256)
                .MustAsync(CheckUniqueConstraint_SingleUniqueField)
                .WithMessage("SingleUniqueField already exists.");

            RuleFor(v => v.CompUniqueFieldA)
                .NotNull()
                .MaximumLength(256);

            RuleFor(v => v.CompUniqueFieldB)
                .NotNull()
                .MaximumLength(256);

            RuleFor(v => v.UniqueConstraintIndexCompositeEntityForStereotypes)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto>()!));

            RuleFor(v => v)
                .MustAsync(CheckUniqueConstraint_CompUniqueFieldA_CompUniqueFieldB)
                .WithMessage("The combination of CompUniqueFieldA and CompUniqueFieldB already exists.");
        }

        private async Task<bool> CheckUniqueConstraint_SingleUniqueField(
            UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand model,
            string value,
            CancellationToken cancellationToken)
        {
            return !await _aggregateWithUniqueConstraintIndexStereotypeRepository.AnyAsync(p => p.Id != model.Id && p.SingleUniqueField == model.SingleUniqueField, cancellationToken);
        }

        private async Task<bool> CheckUniqueConstraint_CompUniqueFieldA_CompUniqueFieldB(
            UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommand model,
            CancellationToken cancellationToken)
        {
            return !await _aggregateWithUniqueConstraintIndexStereotypeRepository.AnyAsync(p => p.Id != model.Id && p.CompUniqueFieldA == model.CompUniqueFieldA && p.CompUniqueFieldB == model.CompUniqueFieldB, cancellationToken);
        }
    }
}