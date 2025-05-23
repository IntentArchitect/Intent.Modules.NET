using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryNoInputDtoReturnCollection
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryNoInputDtoReturnCollectionValidator : AbstractValidator<QueryNoInputDtoReturnCollection>
    {
        [IntentManaged(Mode.Merge)]
        public QueryNoInputDtoReturnCollectionValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}