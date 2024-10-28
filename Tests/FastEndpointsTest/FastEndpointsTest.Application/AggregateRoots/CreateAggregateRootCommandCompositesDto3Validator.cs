using FastEndpointsTest.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootCommandCompositesDto3Validator : AbstractValidator<CreateAggregateRootCommandCompositesDto3>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCommandCompositesDto3Validator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateAggregateRootCommandCompositesDto4>()!));
        }
    }
}