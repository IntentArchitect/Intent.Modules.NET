using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS.UpdateAggregateRootCompositeManyB
{
    public class UpdateAggregateRootCompositeManyBCommand : IRequest, ICommand
    {
        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime? SomeDate { get; set; }

    }
}