using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.CreateDb1Entity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDb1EntityCommandValidator : AbstractValidator<CreateDb1EntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDb1EntityCommandValidator()
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