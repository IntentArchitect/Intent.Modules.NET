using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.CreateAggregateRoot
{
    public class CreateAggregateRootCommand : IRequest<Guid>, ICommand
    {
        public string AggregateAttr { get; set; }

        public CreateAggregateRootCompositeSingleADTO? Composite { get; set; }

        public List<CreateAggregateRootCompositeManyBDTO> Composites { get; set; }

        public CreateAggregateRootAggregateSingleCDTO? Aggregate { get; set; }

    }
}