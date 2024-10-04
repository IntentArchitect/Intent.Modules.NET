using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.People.UpdatePerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePersonCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Details)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdatePersonDetailsDto>()!);
        }
    }
}