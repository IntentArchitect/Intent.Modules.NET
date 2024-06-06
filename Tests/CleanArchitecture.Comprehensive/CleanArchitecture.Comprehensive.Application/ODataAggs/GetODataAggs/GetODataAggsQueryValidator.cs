using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetODataAggsQueryValidator : AbstractValidator<GetODataAggsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetODataAggsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}