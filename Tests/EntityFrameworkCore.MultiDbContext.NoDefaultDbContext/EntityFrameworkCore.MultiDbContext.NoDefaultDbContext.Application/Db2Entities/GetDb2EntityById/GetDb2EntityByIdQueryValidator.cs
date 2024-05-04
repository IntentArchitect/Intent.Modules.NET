using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.GetDb2EntityById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDb2EntityByIdQueryValidator : AbstractValidator<GetDb2EntityByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDb2EntityByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}