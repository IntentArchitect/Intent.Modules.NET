using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.GetDb1Entities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDb1EntitiesQueryValidator : AbstractValidator<GetDb1EntitiesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDb1EntitiesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}