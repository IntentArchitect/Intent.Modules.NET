using System.Collections.Generic;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.GetAggregateRootLongs
{
    public class GetAggregateRootLongsQuery : IRequest<List<AggregateRootLongDto>>, IQuery
    {
        public GetAggregateRootLongsQuery()
        {
        }
    }
}