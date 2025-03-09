using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DictionaryTestDtoValidator : AbstractValidator<DictionaryTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public DictionaryTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DictionaryField)
                .NotNull();

            RuleFor(v => v.DictionaryFieldCollection)
                .NotNull();
        }
    }
}