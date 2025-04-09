using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People.SetPersons
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SetPersonsCommandValidator : AbstractValidator<SetPersonsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SetPersonsCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.People)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PersonDCDto>()!));
        }
    }
}