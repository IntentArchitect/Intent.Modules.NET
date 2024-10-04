using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddContactDetailsVODtoValidator : AbstractValidator<AddContactDetailsVODto>
    {
        [IntentManaged(Mode.Merge)]
        public AddContactDetailsVODtoValidator()
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