using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot
{
    public class UpdateAggregateRootCommand : IRequest, ICommand
    {
        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public List<UpdateAggregateRootCompositeManyBDto> Composites { get; set; }

        public UpdateAggregateRootCompositeSingleADto? Composite { get; set; }

        public UpdateAggregateRootAggregateSingleCDto? Aggregate { get; set; }

    }
}