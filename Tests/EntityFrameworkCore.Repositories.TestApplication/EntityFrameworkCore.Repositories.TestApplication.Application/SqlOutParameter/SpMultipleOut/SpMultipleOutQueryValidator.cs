using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.SqlOutParameter.SpMultipleOut
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SpMultipleOutQueryValidator : AbstractValidator<SpMultipleOutQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SpMultipleOutQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}