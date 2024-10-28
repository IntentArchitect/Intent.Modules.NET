using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.GetAggregateWithUniqueConstraintIndexElementById
{
    public class GetAggregateWithUniqueConstraintIndexElementByIdQuery : IRequest<AggregateWithUniqueConstraintIndexElementDto>, IQuery
    {
        public GetAggregateWithUniqueConstraintIndexElementByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}