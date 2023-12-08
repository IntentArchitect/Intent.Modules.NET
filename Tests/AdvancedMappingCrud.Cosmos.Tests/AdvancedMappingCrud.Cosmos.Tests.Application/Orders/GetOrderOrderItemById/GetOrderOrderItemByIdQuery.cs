using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.GetOrderOrderItemById
{
    public class GetOrderOrderItemByIdQuery : IRequest<OrderOrderItemDto>, IQuery
    {
        public GetOrderOrderItemByIdQuery(string orderId, string id)
        {
            OrderId = orderId;
            Id = id;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
    }
}