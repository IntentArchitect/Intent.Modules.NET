using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.CreateEntityDefault
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEntityDefaultCommandValidator : AbstractValidator<CreateEntityDefaultCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEntityDefaultCommandValidator()
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