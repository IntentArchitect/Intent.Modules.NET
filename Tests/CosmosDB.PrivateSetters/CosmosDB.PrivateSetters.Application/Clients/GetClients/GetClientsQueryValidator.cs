using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.GetClients
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsQueryValidator : AbstractValidator<GetClientsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}