using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.Tags
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TagCreateDtoValidator : AbstractValidator<TagCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public TagCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();
        }
    }
}