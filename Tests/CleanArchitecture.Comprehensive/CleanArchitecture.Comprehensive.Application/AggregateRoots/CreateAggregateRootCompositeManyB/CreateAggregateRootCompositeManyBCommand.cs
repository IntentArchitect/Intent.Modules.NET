using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    public class CreateAggregateRootCompositeManyBCommand : IRequest<Guid>, ICommand
    {
        public CreateAggregateRootCompositeManyBCommand(Guid aggregateRootId,
            string compositeAttr,
            DateTime? someDate,
            CreateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<CreateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            AggregateRootId = aggregateRootId;
            CompositeAttr = compositeAttr;
            SomeDate = someDate;
            Composite = composite;
            Composites = composites;
        }

        public Guid AggregateRootId { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public CreateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        public List<CreateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }
    }
}