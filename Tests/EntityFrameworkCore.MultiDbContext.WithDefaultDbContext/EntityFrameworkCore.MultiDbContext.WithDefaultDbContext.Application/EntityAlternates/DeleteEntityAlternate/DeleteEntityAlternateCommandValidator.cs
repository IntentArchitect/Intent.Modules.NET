using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.DeleteEntityAlternate
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEntityAlternateCommandValidator : AbstractValidator<DeleteEntityAlternateCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEntityAlternateCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}