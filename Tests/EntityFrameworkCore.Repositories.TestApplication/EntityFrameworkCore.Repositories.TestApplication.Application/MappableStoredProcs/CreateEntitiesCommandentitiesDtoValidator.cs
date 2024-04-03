using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEntitiesCommandentitiesDtoValidator : AbstractValidator<CreateEntitiesCommandentitiesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEntitiesCommandentitiesDtoValidator()
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