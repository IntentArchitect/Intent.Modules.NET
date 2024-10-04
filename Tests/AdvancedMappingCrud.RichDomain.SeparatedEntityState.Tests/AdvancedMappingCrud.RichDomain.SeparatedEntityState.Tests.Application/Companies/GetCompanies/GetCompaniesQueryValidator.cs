using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Companies.GetCompanies
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCompaniesQueryValidator : AbstractValidator<GetCompaniesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCompaniesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}