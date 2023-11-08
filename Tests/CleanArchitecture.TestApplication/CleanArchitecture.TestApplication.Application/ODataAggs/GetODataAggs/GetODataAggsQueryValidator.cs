using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ODataAggs.GetODataAggs
{
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