using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateFuneralCoverQuote
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateFuneralCoverQuoteCommandValidator : AbstractValidator<CreateFuneralCoverQuoteCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateFuneralCoverQuoteCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();

            RuleFor(v => v.QuoteLines)
                .NotNull();
        }
    }
}