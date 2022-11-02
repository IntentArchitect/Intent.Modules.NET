using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.CreateAggregateRootA
{
    public class CreateAggregateRootACommand : IRequest<Guid>, ICommand
    {
        public string AggregateAttr { get; set; }

        public CompositeSingleAADTO? Composite { get; set; }

        public List<CompositeManyAADTO> Composites { get; set; }

        public AggregateSingleAADTO? Aggregate { get; set; }

    }
}