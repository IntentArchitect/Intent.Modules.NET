using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.UpdateDb2Entity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDb2EntityCommandValidator : AbstractValidator<UpdateDb2EntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDb2EntityCommandValidator()
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