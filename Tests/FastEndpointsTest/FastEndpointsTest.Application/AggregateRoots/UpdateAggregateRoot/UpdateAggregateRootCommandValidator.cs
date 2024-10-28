using FastEndpointsTest.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRoots.UpdateAggregateRoot
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootCommandValidator : AbstractValidator<UpdateAggregateRootCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.AggregateAttr)
                .NotNull();

            RuleFor(v => v.LimitedDomain)
                .NotNull()
                .MaximumLength(10);

            RuleFor(v => v.LimitedService)
                .NotNull();

            RuleFor(v => v.EnumType1)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumType2)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumType3)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateAggregateRootCommandCompositesDto3>()!));
        }
    }
}