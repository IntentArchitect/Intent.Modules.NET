using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.GetEntityDefaults
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityDefaultsQueryValidator : AbstractValidator<GetEntityDefaultsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityDefaultsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}