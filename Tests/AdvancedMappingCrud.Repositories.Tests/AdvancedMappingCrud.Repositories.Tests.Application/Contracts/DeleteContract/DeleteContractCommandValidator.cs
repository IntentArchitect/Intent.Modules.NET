using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.DeleteContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteContractCommandValidator : AbstractValidator<DeleteContractCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteContractCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}