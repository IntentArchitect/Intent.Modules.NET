using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.DeleteEntityAppDefault
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEntityAppDefaultCommandValidator : AbstractValidator<DeleteEntityAppDefaultCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEntityAppDefaultCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}