using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PersonCreatePersonPersonDetailsNameDtoValidator : AbstractValidator<PersonCreatePersonPersonDetailsNameDto>
    {
        [IntentManaged(Mode.Merge)]
        public PersonCreatePersonPersonDetailsNameDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.First)
                .NotNull();

            RuleFor(v => v.Last)
                .NotNull();
        }
    }
}