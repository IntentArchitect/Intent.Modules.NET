using FluentValidation;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateUniqueAccountEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUniqueAccountEntityCommandValidator : AbstractValidator<CreateUniqueAccountEntityCommand>
    {
        private readonly IUniqueAccountEntityRepository _uniqueAccountEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateUniqueAccountEntityCommandValidator(IUniqueAccountEntityRepository uniqueAccountEntityRepository)
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

        private async Task<bool> CheckUniqueConstraint_Username(string value, CancellationToken cancellationToken)
        {
            return !await _uniqueAccountEntityRepository.AnyAsync(p => p.Username == value, cancellationToken);
        }

        private async Task<bool> CheckUniqueConstraint_Email(string value, CancellationToken cancellationToken)
        {
            return !await _uniqueAccountEntityRepository.AnyAsync(p => p.Email == value, cancellationToken);
        }
    }
}