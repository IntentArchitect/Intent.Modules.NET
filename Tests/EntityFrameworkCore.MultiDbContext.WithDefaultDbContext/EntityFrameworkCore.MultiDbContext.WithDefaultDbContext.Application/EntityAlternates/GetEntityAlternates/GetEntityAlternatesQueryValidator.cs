using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternates
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityAlternatesQueryValidator : AbstractValidator<GetEntityAlternatesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityAlternatesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}