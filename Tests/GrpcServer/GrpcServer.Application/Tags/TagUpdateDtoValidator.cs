using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.Tags
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TagUpdateDtoValidator : AbstractValidator<TagUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public TagUpdateDtoValidator()
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