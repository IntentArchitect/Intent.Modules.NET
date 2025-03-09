using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BinaryTestDtoValidator : AbstractValidator<BinaryTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public BinaryTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.BinaryField)
                .NotNull();

            RuleFor(v => v.BinaryFieldCollection)
                .NotNull();
        }
    }
}