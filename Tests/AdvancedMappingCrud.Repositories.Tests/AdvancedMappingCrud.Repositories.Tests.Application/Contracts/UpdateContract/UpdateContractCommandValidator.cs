using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.UpdateContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateContractCommandValidator()
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