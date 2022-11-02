using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.CreateA_AggregateRoot
{
    public class CreateA_AggregateRootCommand : IRequest<Guid>, ICommand
    {
        public string AggregateAttr { get; set; }

        public A_Composite_SingleDTO Composite { get; set; }

        public List<A_Composite_ManyDTO> Composites { get; set; }

        public A_Aggregation_SingleDTO Aggregation { get; set; }

        public List<A_Aggregation_ManyDTO> Aggregations { get; set; }

    }
}