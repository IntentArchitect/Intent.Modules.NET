using System;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(string id, string warehouseId, string refNo, DateTime orderDate)
        {
            Id = id;
            WarehouseId = warehouseId;
            RefNo = refNo;
            OrderDate = orderDate;
        }

        public string Id { get; set; }
        public string WarehouseId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
    }
}