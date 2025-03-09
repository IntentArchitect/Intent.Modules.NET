using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GuidTestDtoValidator : AbstractValidator<GuidTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public GuidTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.GuidFieldCollection)
                .NotNull();
        }
    }
}