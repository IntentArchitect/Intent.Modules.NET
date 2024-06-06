using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongById
{
    public class GetAggregateRootLongByIdQuery : IRequest<AggregateRootLongDto>, IQuery
    {
        public GetAggregateRootLongByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}