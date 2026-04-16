using FluentValidation;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniqueAccountEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateUniqueAccountEntityCommandValidator : AbstractValidator<UpdateUniqueAccountEntityCommand>
    {
        private readonly IUniqueAccountEntityRepository _uniqueAccountEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateUniqueAccountEntityCommandValidator(IUniqueAccountEntityRepository uniqueAccountEntityRepository)
        {
            ConfigureValidationRules();
            _uniqueAccountEntityRepository = uniqueAccountEntityRepository;
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Username)
                .NotNull()
                .MustAsync(CheckUniqueConstraint_Username)
                .WithMessage("Username already exists.");

            RuleFor(v => v.Email)
                .NotNull()
                .MustAsync(CheckUniqueConstraint_Email)
                .WithMessage("Email already exists.");
        }

        private async Task<bool> CheckUniqueConstraint_Username(
            UpdateUniqueAccountEntityCommand model,
            string value,
            CancellationToken cancellationToken)
        {
            return !await _uniqueAccountEntityRepository.AnyAsync(p => p.Id != model.Id && p.Username == model.Username, cancellationToken);
        }

        private async Task<bool> CheckUniqueConstraint_Email(
            UpdateUniqueAccountEntityCommand model,
            string value,
            CancellationToken cancellationToken)
        {
            return !await _uniqueAccountEntityRepository.AnyAsync(p => p.Id != model.Id && p.Email == model.Email, cancellationToken);
        }
    }
}