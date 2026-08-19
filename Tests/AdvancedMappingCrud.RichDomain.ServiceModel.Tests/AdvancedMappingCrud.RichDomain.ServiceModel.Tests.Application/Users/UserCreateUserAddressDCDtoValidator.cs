using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UserCreateUserAddressDCDtoValidator : AbstractValidator<UserCreateUserAddressDCDto>
    {
        [IntentManaged(Mode.Merge)]
        public UserCreateUserAddressDCDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.Line2)
                .NotNull();

            RuleFor(v => v.City)
                .NotNull();
        }
    }
}