using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.CreateDb2Entity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDb2EntityCommandValidator : AbstractValidator<CreateDb2EntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDb2EntityCommandValidator()
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