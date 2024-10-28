using System.Collections.Generic;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRoots
{
    public class GetAggregateRootsQuery : IRequest<List<AggregateRootDto>>, IQuery
    {
        public GetAggregateRootsQuery()
        {
        }
    }
}