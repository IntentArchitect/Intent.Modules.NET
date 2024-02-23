using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.UpdateCorporateFuneralCoverQuote
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCorporateFuneralCoverQuoteCommandValidator : AbstractValidator<UpdateCorporateFuneralCoverQuoteCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCorporateFuneralCoverQuoteCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();

            RuleFor(v => v.QuoteLines)
                .NotNull();

            RuleFor(v => v.Corporate)
                .NotNull();

            RuleFor(v => v.Registration)
                .NotNull();
        }
    }
}