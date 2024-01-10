using System;
using System.Collections.Generic;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(Guid id, string refNo, List<UpdateOrderOrderItemDto> orderItems)
        {
            Id = id;
            RefNo = refNo;
            OrderItems = orderItems;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public List<UpdateOrderOrderItemDto> OrderItems { get; set; }
    }
}