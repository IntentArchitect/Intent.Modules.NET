using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong
{
    public class CreateAggregateRootLongCommand : IRequest<long>, ICommand
    {
        public string Attribute { get; set; }

        public CreateCompositeOfAggrLongDTO? CompositeOfAggrLong { get; set; }
    }
}