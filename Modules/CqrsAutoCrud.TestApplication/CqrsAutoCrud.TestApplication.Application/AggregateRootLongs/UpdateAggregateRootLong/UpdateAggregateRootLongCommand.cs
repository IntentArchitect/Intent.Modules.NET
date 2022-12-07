using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    public class UpdateAggregateRootLongCommand : IRequest, ICommand
    {
        public long Id { get; set; }

        public UpdateAggregateRootLongCompositeOfAggrLongDTO? CompositeOfAggrLong { get; set; }

        public string Attribute { get; set; }

    }
}