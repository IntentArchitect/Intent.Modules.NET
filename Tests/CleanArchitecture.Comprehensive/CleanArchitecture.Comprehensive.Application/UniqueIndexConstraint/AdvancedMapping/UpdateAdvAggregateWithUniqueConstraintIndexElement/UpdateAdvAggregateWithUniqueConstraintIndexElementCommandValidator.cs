using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.UpdateAdvAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAdvAggregateWithUniqueConstraintIndexElementCommandValidator : AbstractValidator<UpdateAdvAggregateWithUniqueConstraintIndexElementCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateAdvAggregateWithUniqueConstraintIndexElementCommandValidator(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository)
        {
            ConfigureValidationRules();
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
        }

        private void ConfigureValidationRules()
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

            RuleFor(v => v)
                .MustAsync(CheckUniqueConstraint_CompUniqueFieldA_CompUniqueFieldB)
                .WithMessage("The combination of CompUniqueFieldA and CompUniqueFieldB already exists.");
        }

        private async Task<bool> CheckUniqueConstraint_SingleUniqueField(
            UpdateAdvAggregateWithUniqueConstraintIndexElementCommand model,
            string value,
            CancellationToken cancellationToken)
        {
            return !await _aggregateWithUniqueConstraintIndexElementRepository.AnyAsync(p => p.Id != model.Id && p.SingleUniqueField == model.SingleUniqueField, cancellationToken);
        }

        private async Task<bool> CheckUniqueConstraint_CompUniqueFieldA_CompUniqueFieldB(
            UpdateAdvAggregateWithUniqueConstraintIndexElementCommand model,
            CancellationToken cancellationToken)
        {
            return !await _aggregateWithUniqueConstraintIndexElementRepository.AnyAsync(p => p.Id != model.Id && p.CompUniqueFieldA == model.CompUniqueFieldA && p.CompUniqueFieldB == model.CompUniqueFieldB, cancellationToken);
        }
    }
}