using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerCommandPreferencesDtoValidator : AbstractValidator<CreateCustomerCommandPreferencesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerCommandPreferencesDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Theme)
                .NotNull()
                .IsInEnum();
        }
    }
}