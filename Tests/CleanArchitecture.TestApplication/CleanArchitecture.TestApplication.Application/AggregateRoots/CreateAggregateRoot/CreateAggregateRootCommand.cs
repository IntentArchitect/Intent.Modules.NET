using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot
{
    public class CreateAggregateRootCommand : IRequest, ICommand
    {
        public string AggregateAttr { get; set; }

        public List<CreateAggregateRootCompositeManyBDto> Composites { get; set; }

        public CreateAggregateRootCompositeSingleADto? Composite { get; set; }

        public CreateAggregateRootAggregateSingleCDto? Aggregate { get; set; }

    }
}