using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggsWithSelect
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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