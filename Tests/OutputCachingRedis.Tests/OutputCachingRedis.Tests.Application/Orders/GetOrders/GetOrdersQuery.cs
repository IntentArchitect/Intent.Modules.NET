using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Orders.GetOrders
{
    public class GetOrdersQuery : IRequest<List<OrderDto>>, IQuery
    {
        public GetOrdersQuery()
        {
        }
    }
}