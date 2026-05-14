using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.DeleteCustomerSegments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCustomerSegmentsCommandValidator : AbstractValidator<DeleteCustomerSegmentsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCustomerSegmentsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}