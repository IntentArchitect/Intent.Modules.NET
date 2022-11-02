using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.GetA_AggregateRootById
{
    public class GetA_AggregateRootByIdQuery : IRequest<A_AggregateRootDTO>, IQuery
    {
        public Guid Id { get; set; }

    }
}