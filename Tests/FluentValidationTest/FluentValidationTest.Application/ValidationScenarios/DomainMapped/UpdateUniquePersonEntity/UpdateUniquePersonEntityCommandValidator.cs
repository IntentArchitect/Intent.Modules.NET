using FluentValidation;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniquePersonEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateUniquePersonEntityCommandValidator : AbstractValidator<UpdateUniquePersonEntityCommand>
    {
        private readonly IUniquePersonEntityRepository _uniquePersonEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateUniquePersonEntityCommandValidator(IUniquePersonEntityRepository uniquePersonEntityRepository)
        {
            ConfigureValidationRules();
            _uniquePersonEntityRepository = uniquePersonEntityRepository;
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FirstName)
                .NotNull();

            RuleFor(v => v.LastName)
                .NotNull();

            RuleFor(v => v)
                .MustAsync(CheckUniqueConstraint_FirstName_LastName)
                .WithMessage("The combination of FirstName and LastName already exists.");
        }

        private async Task<bool> CheckUniqueConstraint_FirstName_LastName(
            UpdateUniquePersonEntityCommand model,
            CancellationToken cancellationToken)
        {
            return !await _uniquePersonEntityRepository.AnyAsync(p => p.Id != model.Id && p.FirstName == model.FirstName && p.LastName == model.LastName, cancellationToken);
        }
    }
}