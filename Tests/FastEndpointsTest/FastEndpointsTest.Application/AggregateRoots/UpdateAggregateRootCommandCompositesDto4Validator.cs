using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootCommandCompositesDto4Validator : AbstractValidator<UpdateAggregateRootCommandCompositesDto4>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCommandCompositesDto4Validator()
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