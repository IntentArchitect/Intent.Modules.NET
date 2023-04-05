using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    public class CreateAggregateRootCompositeManyBCommand : IRequest<Guid>, ICommand
    {
        public Guid AggregateRootId { get; set; }

        public string CompositeAttr { get; set; }

        public DateTime? SomeDate { get; set; }

        public CreateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }

        public List<CreateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

    }
}