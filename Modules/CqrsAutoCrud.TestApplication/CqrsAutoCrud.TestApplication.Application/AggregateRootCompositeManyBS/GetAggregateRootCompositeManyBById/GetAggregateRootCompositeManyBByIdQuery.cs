using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS.GetAggregateRootCompositeManyBById
{
    public class GetAggregateRootCompositeManyBByIdQuery : IRequest<AggregateRootCompositeManyBDTO>, IQuery
    {
        public Guid AggregateRootId { get; set; }

        public Guid Id { get; set; }

        public CompositeSingleBBDTO? Composite { get; set; }

        public List<CompositeManyBBDTO> Composites { get; set; }

    }
}