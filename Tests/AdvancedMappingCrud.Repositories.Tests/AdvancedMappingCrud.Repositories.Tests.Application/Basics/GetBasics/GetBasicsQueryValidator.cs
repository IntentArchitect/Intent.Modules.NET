using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasics
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBasicsQueryValidator : AbstractValidator<GetBasicsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBasicsQueryValidator()
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