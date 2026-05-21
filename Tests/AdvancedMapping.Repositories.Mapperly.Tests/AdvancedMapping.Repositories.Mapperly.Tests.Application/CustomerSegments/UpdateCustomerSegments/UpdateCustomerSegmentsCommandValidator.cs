using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.UpdateCustomerSegments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerSegmentsCommandValidator : AbstractValidator<UpdateCustomerSegmentsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerSegmentsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}