using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DateTimeOffsetTestDtoValidator : AbstractValidator<DateTimeOffsetTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public DateTimeOffsetTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DateTimeOffsetFieldCollection)
                .NotNull();
        }
    }
}