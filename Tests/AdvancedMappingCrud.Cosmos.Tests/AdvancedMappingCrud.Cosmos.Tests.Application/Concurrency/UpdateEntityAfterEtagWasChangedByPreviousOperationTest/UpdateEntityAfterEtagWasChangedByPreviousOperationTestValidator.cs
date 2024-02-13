using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Concurrency.UpdateEntityAfterEtagWasChangedByPreviousOperationTest
{
    public class UpdateEntityAfterEtagWasChangedByPreviousOperationTestValidator : AbstractValidator<UpdateEntityAfterEtagWasChangedByPreviousOperationTest>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEntityAfterEtagWasChangedByPreviousOperationTestValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}