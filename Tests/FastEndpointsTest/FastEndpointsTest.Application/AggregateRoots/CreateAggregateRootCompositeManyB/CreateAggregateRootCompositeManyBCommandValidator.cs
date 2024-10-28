using FastEndpointsTest.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootCompositeManyBCommandValidator : AbstractValidator<CreateAggregateRootCompositeManyBCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCompositeManyBCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateAggregateRootCompositeManyBCommandCompositesDto>()!));

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetValidator<CreateAggregateRootCompositeManyBCommandCompositeDto>()!);
        }
    }
}