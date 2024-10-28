using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootById
{
    public class GetAggregateRootByIdQuery : IRequest<AggregateRootDto>, IQuery
    {
        public GetAggregateRootByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}