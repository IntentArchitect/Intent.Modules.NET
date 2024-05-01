using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.CreateContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateContractCommandValidator()
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