using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.GetAggregateWithUniqueConstraintIndexStereotypeById
{
    public class GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery : IRequest<AggregateWithUniqueConstraintIndexStereotypeDto>, IQuery
    {
        public GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}