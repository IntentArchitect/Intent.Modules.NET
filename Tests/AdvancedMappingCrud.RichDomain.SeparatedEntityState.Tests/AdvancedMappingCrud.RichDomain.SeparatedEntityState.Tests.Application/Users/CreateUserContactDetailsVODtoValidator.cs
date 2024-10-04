using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserContactDetailsVODtoValidator : AbstractValidator<CreateUserContactDetailsVODto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserContactDetailsVODtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Cell)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}