using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.UpdateDb1Entity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDb1EntityCommandValidator : AbstractValidator<UpdateDb1EntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDb1EntityCommandValidator()
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