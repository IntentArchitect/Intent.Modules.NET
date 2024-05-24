using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.AddCoolectionsUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddCoolectionsUserCommandValidator : AbstractValidator<AddCoolectionsUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public AddCoolectionsUserCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Addresses)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<AddAddressDCDto>()!));

            RuleFor(v => v.Contacts)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<AddContactDetailsVODto>()!));
        }
    }
}