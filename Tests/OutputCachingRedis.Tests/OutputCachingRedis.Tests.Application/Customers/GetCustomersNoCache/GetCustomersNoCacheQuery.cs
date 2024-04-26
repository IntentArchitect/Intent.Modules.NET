using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Customers.GetCustomersNoCache
{
    public class GetCustomersNoCacheQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersNoCacheQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; set; }
    }
}