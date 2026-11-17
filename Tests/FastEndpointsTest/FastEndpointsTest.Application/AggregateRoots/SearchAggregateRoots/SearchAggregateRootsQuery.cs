using System.Collections.Generic;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.SearchAggregateRoots
{
    /// <summary>
    /// HTTP QUERY endpoint: the search criteria travel in the request body.
    /// </summary>
    public class SearchAggregateRootsQuery : IRequest<List<AggregateRootDto>>, IQuery
    {
        public SearchAggregateRootsQuery(string aggregateAttr)
        {
            AggregateAttr = aggregateAttr;
        }

        public string AggregateAttr { get; set; }
    }
}