using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.DeleteEntityDefault
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEntityDefaultCommandValidator : AbstractValidator<DeleteEntityDefaultCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEntityDefaultCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}