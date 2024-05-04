using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaultById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityAppDefaultByIdQueryValidator : AbstractValidator<GetEntityAppDefaultByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityAppDefaultByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}