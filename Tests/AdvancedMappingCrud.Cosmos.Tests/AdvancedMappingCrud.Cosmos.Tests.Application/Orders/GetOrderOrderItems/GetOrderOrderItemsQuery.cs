using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.GetOrderOrderItems
{
    public class GetOrderOrderItemsQuery : IRequest<List<OrderOrderItemDto>>, IQuery
    {
        public GetOrderOrderItemsQuery(string orderId)
        {
            OrderId = orderId;
        }

        public string OrderId { get; set; }
    }
}