using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild3
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateChild3CommandValidator : AbstractValidator<CreateChild3Command>
    {
        [IntentManaged(Mode.Merge)]
        public CreateChild3CommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Dto)
                .NotNull()
                .SetValidator(provider.GetValidator<Child3>()!);
        }
    }
}