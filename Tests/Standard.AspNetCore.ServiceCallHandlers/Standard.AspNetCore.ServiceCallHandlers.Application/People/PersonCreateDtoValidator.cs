using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.People
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PersonCreateDtoValidator : AbstractValidator<PersonCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public PersonCreateDtoValidator()
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