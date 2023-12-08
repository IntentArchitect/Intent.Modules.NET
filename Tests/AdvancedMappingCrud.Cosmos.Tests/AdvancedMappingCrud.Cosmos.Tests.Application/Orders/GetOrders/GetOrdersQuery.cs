using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.GetOrders
{
    public class GetOrdersQuery : IRequest<List<OrderDto>>, IQuery
    {
        public GetOrdersQuery()
        {
        }
    }
}