using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RecordOrderCommandValidator : AbstractValidator<RecordOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RecordOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CustomerName)
                .NotNull();
        }
    }
}