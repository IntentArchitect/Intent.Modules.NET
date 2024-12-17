using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.GetClientsFiltered
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsFilteredQueryValidator : AbstractValidator<GetClientsFilteredQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsFilteredQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Type)
                .IsInEnum();
        }
    }
}