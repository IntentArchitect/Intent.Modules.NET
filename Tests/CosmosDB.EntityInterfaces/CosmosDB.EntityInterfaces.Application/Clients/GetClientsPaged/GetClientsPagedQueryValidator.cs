using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Clients.GetClientsPaged
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsPagedQueryValidator : AbstractValidator<GetClientsPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsPagedQueryValidator()
        {
        }
    }
}