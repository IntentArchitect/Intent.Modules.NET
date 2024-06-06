using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRoot
{
    public class CreateAggregateRootCommand : IRequest<Guid>, ICommand
    {
        public CreateAggregateRootCommand(string aggregateAttr,
            List<CreateAggregateRootCompositeManyBDto> composites,
            CreateAggregateRootCompositeSingleADto? composite,
            CreateAggregateRootAggregateSingleCDto? aggregate,
            string limitedDomain,
            string limitedService)
        {
            AggregateAttr = aggregateAttr;
            Composites = composites;
            Composite = composite;
            Aggregate = aggregate;
            LimitedDomain = limitedDomain;
            LimitedService = limitedService;
        }

        public string AggregateAttr { get; set; }
        public List<CreateAggregateRootCompositeManyBDto> Composites { get; set; }
        public CreateAggregateRootCompositeSingleADto? Composite { get; set; }
        public CreateAggregateRootAggregateSingleCDto? Aggregate { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }
    }
}