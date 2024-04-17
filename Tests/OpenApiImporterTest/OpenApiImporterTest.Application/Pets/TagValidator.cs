using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TagValidator : AbstractValidator<Tag>
    {
        [IntentManaged(Mode.Merge)]
        public TagValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}