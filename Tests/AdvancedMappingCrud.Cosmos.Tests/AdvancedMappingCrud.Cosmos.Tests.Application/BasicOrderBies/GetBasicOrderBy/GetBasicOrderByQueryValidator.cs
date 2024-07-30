using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.GetBasicOrderBy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBasicOrderByQueryValidator : AbstractValidator<GetBasicOrderByQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBasicOrderByQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderBy)
                .NotNull();
        }
    }
}