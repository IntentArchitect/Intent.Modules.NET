using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ObjectTestDtoValidator : AbstractValidator<ObjectTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public ObjectTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ObjectField)
                .NotNull();

            RuleFor(v => v.ObjectFieldCollection)
                .NotNull();
        }
    }
}