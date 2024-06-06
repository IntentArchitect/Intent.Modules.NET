using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggsWithSelect
{
    public class GetODataAggsWithSelectQuery : IRequest<IEnumerable>, IQuery
    {
        public GetODataAggsWithSelectQuery(Func<IQueryable<ODataAggDto>, IQueryable> transform)
        {
            Transform = transform;
        }

        public Func<IQueryable<ODataAggDto>, IQueryable> Transform { get; }
    }
}