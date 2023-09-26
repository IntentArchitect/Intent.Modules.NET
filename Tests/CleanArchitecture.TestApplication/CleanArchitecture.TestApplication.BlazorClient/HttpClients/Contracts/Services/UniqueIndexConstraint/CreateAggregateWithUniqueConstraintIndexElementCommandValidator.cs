using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.UniqueIndexConstraint
{
    public class CreateAggregateWithUniqueConstraintIndexElementCommandValidator : AbstractValidator<CreateAggregateWithUniqueConstraintIndexElementCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateAggregateWithUniqueConstraintIndexElementCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
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
        }
    }
}