using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StringTestDtoValidator : AbstractValidator<StringTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public StringTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.StringField)
                .NotNull();

            RuleFor(v => v.StringFieldCollection)
                .NotNull();
        }
    }
}