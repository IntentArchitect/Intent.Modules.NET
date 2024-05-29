using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggs
{
    public class GetODataAggsQuery : IRequest<List<ODataAggDto>>, IQuery
    {
        public GetODataAggsQuery(Func<IQueryable<ODataAggDto>, IQueryable> transform)
        {
            Transform = transform;
        }

        public Func<IQueryable<ODataAggDto>, IQueryable> Transform { get; }
    }
}