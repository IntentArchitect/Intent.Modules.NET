using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.UniqueIndexConstraint.ClassicMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateViaContructorCommandValidator : AbstractValidator<CreateViaContructorCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateViaContructorCommandValidator()
        {
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task<bool> CheckUniqueConstraint_SingleUniqueField(string value, CancellationToken cancellationToken)
        {
            // Implement custom unique constraint validation here
            return true;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task<bool> CheckUniqueConstraint_CompUniqueFieldA_CompUniqueFieldB(
            CreateViaContructorCommand model,
            CancellationToken cancellationToken)
        {
            // Implement custom unique constraint validation here
            return true;
        }
    }
}