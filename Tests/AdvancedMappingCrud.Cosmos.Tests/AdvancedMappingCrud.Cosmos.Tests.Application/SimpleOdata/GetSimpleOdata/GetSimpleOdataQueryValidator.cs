using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdata
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetSimpleOdataQueryValidator : AbstractValidator<GetSimpleOdataQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetSimpleOdataQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}