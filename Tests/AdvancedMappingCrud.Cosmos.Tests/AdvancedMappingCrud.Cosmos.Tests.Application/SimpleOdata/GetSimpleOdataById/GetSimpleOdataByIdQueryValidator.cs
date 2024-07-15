using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdataById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetSimpleOdataByIdQueryValidator : AbstractValidator<GetSimpleOdataByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetSimpleOdataByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}