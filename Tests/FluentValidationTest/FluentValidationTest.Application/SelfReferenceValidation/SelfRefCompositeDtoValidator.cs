using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SelfRefCompositeDtoValidator : AbstractValidator<SelfRefCompositeDto>
    {
        [IntentManaged(Mode.Merge)]
        public SelfRefCompositeDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Entry)
                .NotNull();
        }
    }
}