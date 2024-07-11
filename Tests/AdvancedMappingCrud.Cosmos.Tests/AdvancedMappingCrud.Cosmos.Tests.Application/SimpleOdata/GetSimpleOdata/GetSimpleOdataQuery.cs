using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdata
{
    public class GetSimpleOdataQuery : IRequest<List<SimpleOdataDto>>, IQuery
    {
        public GetSimpleOdataQuery(Func<IQueryable<SimpleOdataDto>, IQueryable> transform)
        {
            Transform = transform;
        }

        public Func<IQueryable<SimpleOdataDto>, IQueryable> Transform { get; }
    }
}