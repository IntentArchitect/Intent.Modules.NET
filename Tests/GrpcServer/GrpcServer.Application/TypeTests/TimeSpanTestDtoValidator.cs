using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TimeSpanTestDtoValidator : AbstractValidator<TimeSpanTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public TimeSpanTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.TimeSpanFieldCollection)
                .NotNull();
        }
    }
}