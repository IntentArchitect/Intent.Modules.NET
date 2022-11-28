using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.UpdateAggregateRootCompositeMany
{
    public class UpdateAggregateRootCompositeManyCommand : IRequest, ICommand
    {
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }


        public UpdateCompositeSingleBBDTO? Composite { get; set; }

        public List<UpdateCompositeManyBBDTO> Composites { get; set; }

    }
}