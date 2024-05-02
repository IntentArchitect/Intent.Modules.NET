using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternateById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityAlternateByIdQueryValidator : AbstractValidator<GetEntityAlternateByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityAlternateByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}