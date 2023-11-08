using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ODataAggs.GetODataAggsWithSelect
{
    public class GetODataAggsWithSelectQueryValidator : AbstractValidator<GetODataAggsWithSelectQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetODataAggsWithSelectQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}