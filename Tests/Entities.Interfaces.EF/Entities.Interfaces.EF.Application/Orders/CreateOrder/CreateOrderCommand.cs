using System;
using System.Collections.Generic;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(string refNo, List<CreateOrderOrderItemDto> orderItems)
        {
            RefNo = refNo;
            OrderItems = orderItems;
        }

        public string RefNo { get; set; }
        public List<CreateOrderOrderItemDto> OrderItems { get; set; }
    }
}