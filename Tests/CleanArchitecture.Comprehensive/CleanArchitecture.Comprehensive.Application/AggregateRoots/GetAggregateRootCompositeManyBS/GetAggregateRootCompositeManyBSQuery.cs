using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBS
{
    public class GetAggregateRootCompositeManyBSQuery : IRequest<List<AggregateRootCompositeManyBDto>>, IQuery
    {
        public GetAggregateRootCompositeManyBSQuery(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }

        public Guid AggregateRootId { get; set; }
    }
}