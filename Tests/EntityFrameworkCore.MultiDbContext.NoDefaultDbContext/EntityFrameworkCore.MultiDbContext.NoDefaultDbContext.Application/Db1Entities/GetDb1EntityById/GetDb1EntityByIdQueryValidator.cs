using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.GetDb1EntityById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDb1EntityByIdQueryValidator : AbstractValidator<GetDb1EntityByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDb1EntityByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}