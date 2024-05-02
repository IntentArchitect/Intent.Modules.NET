using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.GetDb2Entities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDb2EntitiesQueryValidator : AbstractValidator<GetDb2EntitiesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDb2EntitiesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}