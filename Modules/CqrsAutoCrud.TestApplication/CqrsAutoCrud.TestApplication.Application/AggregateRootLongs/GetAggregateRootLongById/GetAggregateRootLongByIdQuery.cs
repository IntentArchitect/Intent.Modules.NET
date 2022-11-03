using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongById
{
    public class GetAggregateRootLongByIdQuery : IRequest<AggregateRootLongDTO>, IQuery
    {
        public long Id { get; set; }

    }
}