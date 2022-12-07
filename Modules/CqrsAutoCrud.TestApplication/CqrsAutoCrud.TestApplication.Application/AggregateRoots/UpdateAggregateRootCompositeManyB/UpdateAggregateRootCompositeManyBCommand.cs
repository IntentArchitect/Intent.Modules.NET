using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    public class UpdateAggregateRootCompositeManyBCommand : IRequest, ICommand
    {
        public string CompositeAttr { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime? SomeDate { get; set; }

        public Guid Id { get; set; }

        public UpdateAggregateRootCompositeManyBCompositeSingleBBDTO? Composite { get; set; }

        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDTO> Composites { get; set; }

    }
}