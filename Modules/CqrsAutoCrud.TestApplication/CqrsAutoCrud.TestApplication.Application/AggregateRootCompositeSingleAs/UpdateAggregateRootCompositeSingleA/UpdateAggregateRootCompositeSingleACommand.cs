using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.UpdateAggregateRootCompositeSingleA
{
    public class UpdateAggregateRootCompositeSingleACommand : IRequest, ICommand
    {
        public Guid Id { get; set; }

        public Guid AggregateRootId { get; set; }
        public string CompositeAttr { get; set; }

        public UpdateCompositeSingleAADTO? Composite { get; set; }

        public List<UpdateCompositeManyAADTO> Composites { get; set; }

    }
}