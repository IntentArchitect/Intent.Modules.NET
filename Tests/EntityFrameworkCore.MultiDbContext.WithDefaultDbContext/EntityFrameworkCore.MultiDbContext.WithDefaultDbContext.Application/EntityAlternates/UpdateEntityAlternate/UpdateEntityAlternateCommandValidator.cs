using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.UpdateEntityAlternate
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEntityAlternateCommandValidator : AbstractValidator<UpdateEntityAlternateCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEntityAlternateCommandValidator()
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