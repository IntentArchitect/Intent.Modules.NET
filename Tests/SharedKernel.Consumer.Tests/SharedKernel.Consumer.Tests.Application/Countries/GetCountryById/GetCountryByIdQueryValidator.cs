using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries.GetCountryById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCountryByIdQueryValidator : AbstractValidator<GetCountryByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCountryByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}