using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.DeleteDb1Entity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDb1EntityCommandValidator : AbstractValidator<DeleteDb1EntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDb1EntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}