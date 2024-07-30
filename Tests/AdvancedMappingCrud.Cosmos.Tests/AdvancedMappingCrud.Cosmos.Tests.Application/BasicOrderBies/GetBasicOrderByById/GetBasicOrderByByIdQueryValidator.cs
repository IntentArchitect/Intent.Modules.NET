using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.GetBasicOrderByById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBasicOrderByByIdQueryValidator : AbstractValidator<GetBasicOrderByByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBasicOrderByByIdQueryValidator()
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