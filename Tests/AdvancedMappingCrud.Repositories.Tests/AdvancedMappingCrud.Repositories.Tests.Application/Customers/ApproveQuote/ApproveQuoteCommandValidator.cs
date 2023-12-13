using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.ApproveQuote
{
    public class ApproveQuoteCommandValidator : AbstractValidator<ApproveQuoteCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ApproveQuoteCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}