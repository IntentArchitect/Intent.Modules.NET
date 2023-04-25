using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs
{
    public class GetAggregateRootLongsQuery : IRequest<PagedResult<AggregateRootLongDto>>, IQuery
    {
        public int PageNo { get; set; }

        public int PageSize { get; set; }
    }
}