using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryGuidReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryGuidReturnValidator : AbstractValidator<QueryGuidReturn>
    {
        [IntentManaged(Mode.Merge)]
        public QueryGuidReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Input)
                .NotNull();
        }
    }
}