using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Regions.GetRegionById
{
    public class GetRegionByIdQuery : IRequest<RegionDto>, IQuery
    {
        public GetRegionByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}