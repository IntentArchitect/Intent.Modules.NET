using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.UpdateEntityDefault
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEntityDefaultCommandValidator : AbstractValidator<UpdateEntityDefaultCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEntityDefaultCommandValidator()
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