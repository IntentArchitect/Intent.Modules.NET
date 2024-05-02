using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.DeleteDb2Entity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDb2EntityCommandValidator : AbstractValidator<DeleteDb2EntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDb2EntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}