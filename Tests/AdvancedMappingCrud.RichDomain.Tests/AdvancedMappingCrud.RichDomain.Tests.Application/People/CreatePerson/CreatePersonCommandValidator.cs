using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People.CreatePerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePersonCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Details)
                .NotNull()
                .SetValidator(provider.GetValidator<CreatePersonPersonDetailsDto>()!);
        }
    }
}