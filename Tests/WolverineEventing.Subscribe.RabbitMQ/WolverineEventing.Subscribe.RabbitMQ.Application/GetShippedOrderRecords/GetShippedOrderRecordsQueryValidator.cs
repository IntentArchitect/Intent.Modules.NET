using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.QueryValidator", Version = "2.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.GetShippedOrderRecords
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetShippedOrderRecordsQueryValidator : AbstractValidator<GetShippedOrderRecordsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetShippedOrderRecordsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}