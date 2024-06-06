using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBById
{
    public class GetAggregateRootCompositeManyBByIdQuery : IRequest<AggregateRootCompositeManyBDto>, IQuery
    {
        public GetAggregateRootCompositeManyBByIdQuery(Guid aggregateRootId, Guid id)
        {
            AggregateRootId = aggregateRootId;
            Id = id;
        }

        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
    }
}