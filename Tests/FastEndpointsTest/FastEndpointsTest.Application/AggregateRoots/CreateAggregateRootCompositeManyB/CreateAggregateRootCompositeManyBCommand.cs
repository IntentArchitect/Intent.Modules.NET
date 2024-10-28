using System;
using System.Collections.Generic;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    public class CreateAggregateRootCompositeManyBCommand : IRequest<Guid>, ICommand
    {
        public CreateAggregateRootCompositeManyBCommand(Guid aggregateRootId,
            string compositeAttr,
            DateTime? someDate,
            List<CreateAggregateRootCompositeManyBCommandCompositesDto> composites,
            CreateAggregateRootCompositeManyBCommandCompositeDto? composite)
        {
            AggregateRootId = aggregateRootId;
            CompositeAttr = compositeAttr;
            SomeDate = someDate;
            Composites = composites;
            Composite = composite;
        }

        public Guid AggregateRootId { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public List<CreateAggregateRootCompositeManyBCommandCompositesDto> Composites { get; set; }
        public CreateAggregateRootCompositeManyBCommandCompositeDto? Composite { get; set; }
    }
}