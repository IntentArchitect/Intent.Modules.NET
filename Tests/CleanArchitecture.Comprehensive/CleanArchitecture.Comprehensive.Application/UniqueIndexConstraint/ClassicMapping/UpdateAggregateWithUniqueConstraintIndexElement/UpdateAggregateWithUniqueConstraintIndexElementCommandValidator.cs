using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.UpdateAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateWithUniqueConstraintIndexElementCommandValidator : AbstractValidator<UpdateAggregateWithUniqueConstraintIndexElementCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAggregateWithUniqueConstraintIndexElementCommandValidator(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
            ConfigureValidationRules();
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

        private async Task<bool> CheckUniqueConstraint_CompUniqueFieldA_CompUniqueFieldB(
            UpdateAggregateWithUniqueConstraintIndexElementCommand model,
            CancellationToken cancellationToken)
        {
            return !await _aggregateWithUniqueConstraintIndexElementRepository.AnyAsync(p => p.Id != model.Id && p.CompUniqueFieldA == model.CompUniqueFieldA && p.CompUniqueFieldB == model.CompUniqueFieldB, cancellationToken);
        }

        private async Task<bool> CheckUniqueConstraint_SingleUniqueField(
            UpdateAggregateWithUniqueConstraintIndexElementCommand model,
            string value,
            CancellationToken cancellationToken)
        {
            return !await _aggregateWithUniqueConstraintIndexElementRepository.AnyAsync(p => p.Id != model.Id && p.SingleUniqueField == model.SingleUniqueField, cancellationToken);
        }
    }
}