using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.CreateEntityAlternate
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEntityAlternateCommandValidator : AbstractValidator<CreateEntityAlternateCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEntityAlternateCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}