using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.GetImplicitKeyAggrRootImplicitKeyNestedCompositions
{
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery : IRequest<List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO>>, IQuery
    {
        public Guid ImplicitKeyAggrRootId { get; set; }

    }
}