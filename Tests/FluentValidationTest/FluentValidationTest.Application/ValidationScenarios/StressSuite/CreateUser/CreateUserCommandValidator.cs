using FluentValidation;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.CreateUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserAccountRepository _userAccountRepository;
        [IntentManaged(Mode.Merge)]
        public CreateUserCommandValidator(IUserAccountRepository userAccountRepository)
        {
            ConfigureValidationRules();
            _userAccountRepository = userAccountRepository;
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull()
                .MustAsync(CheckUniqueConstraint_Email)
                .WithMessage("Email already exists.")
                .MaximumLength(255);
        }

        private async Task<bool> CheckUniqueConstraint_Email(string value, CancellationToken cancellationToken)
        {
            return !await _userAccountRepository.AnyAsync(p => p.Email == value, cancellationToken);
        }
    }
}