using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.CreateEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEntitiesCommandValidator : AbstractValidator<CreateEntitiesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEntitiesCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Entities)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateEntitiesCommandentitiesDto>()!));
        }
    }
}