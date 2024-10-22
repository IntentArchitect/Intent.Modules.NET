using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries.GetCountries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCountriesQueryValidator : AbstractValidator<GetCountriesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCountriesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}