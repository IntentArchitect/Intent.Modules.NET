using FluentValidation;

namespace CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder
{
    public class RecordOrderCommandValidator : AbstractValidator<RecordOrderCommand>
    {
        public RecordOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CustomerName)
                .NotNull();
        }
    }
}