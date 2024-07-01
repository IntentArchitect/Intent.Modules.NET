using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.CreateUniqueConVal
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUniqueConValCommandValidator : AbstractValidator<CreateUniqueConValCommand>
    {
        private readonly IUniqueConValRepository _uniqueConValRepository;

        [IntentManaged(Mode.Merge)]
        public CreateUniqueConValCommandValidator(IUniqueConValRepository uniqueConValRepository)
        {
            ConfigureValidationRules();
            _uniqueConValRepository = uniqueConValRepository;
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Att1)
                .NotNull();

            RuleFor(v => v.Att2)
                .NotNull();

            RuleFor(v => v.AttInclude)
                .NotNull();

            RuleFor(v => v)
                .MustAsync(CheckUniqueConstraint_Att1_Att2)
                .WithMessage("The combination of Att1 and Att2 already exists.");
        }

        private async Task<bool> CheckUniqueConstraint_Att1_Att2(
            CreateUniqueConValCommand model,
            CancellationToken cancellationToken)
        {
            return !await _uniqueConValRepository.AnyAsync(p => p.Att1 == model.Att1 && p.Att2 == model.Att2, cancellationToken);
        }
    }
}