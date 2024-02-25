using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDtoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryResponseDtoReturnValidator : AbstractValidator<QueryResponseDtoReturn>
    {
        [IntentManaged(Mode.Merge)]
        public QueryResponseDtoReturnValidator()
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