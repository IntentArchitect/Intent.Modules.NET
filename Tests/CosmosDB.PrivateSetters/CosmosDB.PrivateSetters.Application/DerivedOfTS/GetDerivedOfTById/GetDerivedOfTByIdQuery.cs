using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.DerivedOfTS.GetDerivedOfTById
{
    public class GetDerivedOfTByIdQuery : IRequest<DerivedOfTDto>, IQuery
    {
        public GetDerivedOfTByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}