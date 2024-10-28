using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootCompositeManyBCommandCompositeDtoValidator : AbstractValidator<CreateAggregateRootCompositeManyBCommandCompositeDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCompositeManyBCommandCompositeDtoValidator()
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