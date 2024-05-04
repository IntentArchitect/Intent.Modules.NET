using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaults
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityAppDefaultsQueryValidator : AbstractValidator<GetEntityAppDefaultsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityAppDefaultsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}