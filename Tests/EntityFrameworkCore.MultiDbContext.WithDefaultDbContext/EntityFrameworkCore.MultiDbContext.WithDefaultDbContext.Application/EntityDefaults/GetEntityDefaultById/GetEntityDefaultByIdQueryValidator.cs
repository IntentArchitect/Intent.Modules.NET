using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.GetEntityDefaultById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityDefaultByIdQueryValidator : AbstractValidator<GetEntityDefaultByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityDefaultByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}