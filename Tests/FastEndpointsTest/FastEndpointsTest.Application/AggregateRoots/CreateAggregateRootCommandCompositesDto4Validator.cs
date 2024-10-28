using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootCommandCompositesDto4Validator : AbstractValidator<CreateAggregateRootCommandCompositesDto4>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCommandCompositesDto4Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();
        }
    }
}