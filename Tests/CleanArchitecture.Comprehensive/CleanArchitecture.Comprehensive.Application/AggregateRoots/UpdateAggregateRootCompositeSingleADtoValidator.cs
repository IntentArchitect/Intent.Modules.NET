using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootCompositeSingleADtoValidator : AbstractValidator<UpdateAggregateRootCompositeSingleADto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCompositeSingleADtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetValidator<UpdateAggregateRootCompositeSingleACompositeSingleAADto>()!);

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateAggregateRootCompositeSingleACompositeManyAADto>()!));
        }
    }
}