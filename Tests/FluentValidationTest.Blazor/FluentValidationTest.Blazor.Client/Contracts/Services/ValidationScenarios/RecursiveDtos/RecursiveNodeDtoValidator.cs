using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client.Contracts.Services.ValidationScenarios.RecursiveDtos
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RecursiveNodeDtoValidator : AbstractValidator<RecursiveNodeDto>
    {
        [IntentManaged(Mode.Merge)]
        public RecursiveNodeDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Child)
                .SetValidator(this);
        }
    }
}