using FastEndpointsTest.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.CreateAggregateRootLong
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootLongCommandValidator : AbstractValidator<CreateAggregateRootLongCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootLongCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.CompositeOfAggrLong)
                .SetValidator(provider.GetValidator<CreateAggregateRootLongCommandCompositeOfAggrLongDto>()!);
        }
    }
}