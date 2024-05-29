using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    public class UpdateAggregateRootCompositeManyBCommand : IRequest, ICommand
    {
        public UpdateAggregateRootCompositeManyBCommand(Guid aggregateRootId,
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            AggregateRootId = aggregateRootId;
            Id = id;
            CompositeAttr = compositeAttr;
            SomeDate = someDate;
            Composite = composite;
            Composites = composites;
        }

        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }
    }
}