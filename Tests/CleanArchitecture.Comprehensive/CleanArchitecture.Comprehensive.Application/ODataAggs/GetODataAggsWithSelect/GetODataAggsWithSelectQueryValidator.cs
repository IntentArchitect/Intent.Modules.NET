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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Depends on user code")]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}