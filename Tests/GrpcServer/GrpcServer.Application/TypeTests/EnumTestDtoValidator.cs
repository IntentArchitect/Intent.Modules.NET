using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EnumTestDtoValidator : AbstractValidator<EnumTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public EnumTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.EnumField)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumFieldCollection)
                .NotNull()
                .ForEach(x => x.IsInEnum());

            RuleFor(v => v.EnumFieldNullable)
                .IsInEnum();

            RuleFor(v => v.EnumFieldNullableCollection)
                .ForEach(x => x.IsInEnum());
        }
    }
}