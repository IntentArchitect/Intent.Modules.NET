using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Orders.UpdateOrderItemsOrder
{
    public class UpdateOrderItemsOrderCommand : IRequest, ICommand
    {
        public UpdateOrderItemsOrderCommand(Guid id, List<UpdateOrderItemUpdateDCDto> orderItemDetails)
        {
            Id = id;
            OrderItemDetails = orderItemDetails;
        }

        public Guid Id { get; set; }
        public List<UpdateOrderItemUpdateDCDto> OrderItemDetails { get; set; }
    }
}