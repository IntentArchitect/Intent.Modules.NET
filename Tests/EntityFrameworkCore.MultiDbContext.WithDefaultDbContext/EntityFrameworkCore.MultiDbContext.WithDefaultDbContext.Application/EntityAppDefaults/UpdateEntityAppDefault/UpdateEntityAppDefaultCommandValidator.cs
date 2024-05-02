using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.UpdateEntityAppDefault
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEntityAppDefaultCommandValidator : AbstractValidator<UpdateEntityAppDefaultCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEntityAppDefaultCommandValidator()
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