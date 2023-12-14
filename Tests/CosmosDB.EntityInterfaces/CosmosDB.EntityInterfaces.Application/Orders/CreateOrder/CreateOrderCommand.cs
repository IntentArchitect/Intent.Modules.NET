using System;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderCreatedDto>, ICommand
    {
        public CreateOrderCommand(string warehouseId, string refNo, DateTime orderDate)
        {
            WarehouseId = warehouseId;
            RefNo = refNo;
            OrderDate = orderDate;
        }

        public string WarehouseId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
    }
}