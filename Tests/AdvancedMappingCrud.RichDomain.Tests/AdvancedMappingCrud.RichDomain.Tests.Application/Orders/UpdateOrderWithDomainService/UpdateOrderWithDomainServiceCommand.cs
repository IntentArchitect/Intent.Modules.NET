using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Orders.UpdateOrderWithDomainService
{
    public class UpdateOrderWithDomainServiceCommand : IRequest, ICommand
    {
        public UpdateOrderWithDomainServiceCommand(Guid id, List<UpdateOrderItemUpdateDCDto> orderItemDetails)
        {
            Id = id;
            OrderItemDetails = orderItemDetails;
        }

        public Guid Id { get; set; }
        public List<UpdateOrderItemUpdateDCDto> OrderItemDetails { get; set; }
    }
}