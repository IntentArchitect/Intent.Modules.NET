using CosmosDB.EnumStrings.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.GetRootEntityById
{
    public class GetRootEntityByIdQuery : IRequest<RootEntityDto>, IQuery
    {
        public GetRootEntityByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}