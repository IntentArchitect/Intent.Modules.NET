using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(string refNo, DateTime orderDate, List<CreateOrderOrderItemDto> orderItems)
        {
            RefNo = refNo;
            OrderDate = orderDate;
            OrderItems = orderItems;
        }

        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CreateOrderOrderItemDto> OrderItems { get; set; }
    }
}