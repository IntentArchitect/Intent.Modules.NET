using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.GetOrderOrderItemById
{
    public class GetOrderOrderItemByIdQuery : IRequest<OrderOrderItemDto>, IQuery
    {
        public GetOrderOrderItemByIdQuery(Guid orderId, Guid id)
        {
            OrderId = orderId;
            Id = id;
        }

        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
    }
}