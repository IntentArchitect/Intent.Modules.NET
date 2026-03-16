using FluentValidation;
using FluentValidationTest.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SelfRefWithCompDtoValidator : AbstractValidator<SelfRefWithCompDto>
    {
        [IntentManaged(Mode.Merge)]
        public SelfRefWithCompDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Entry)
                .NotNull();

            RuleFor(v => v.SelfRefDtos)
                .NotNull()
                .ForEach(x => x.SetValidator(this));

            RuleFor(v => v.Composite)
                .NotNull()
                .SetValidator(provider.GetValidator<SelfRefCompositeDto>()!);
        }
    }
}