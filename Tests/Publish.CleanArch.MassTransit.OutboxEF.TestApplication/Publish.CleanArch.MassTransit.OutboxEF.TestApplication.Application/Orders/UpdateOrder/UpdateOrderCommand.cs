using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(Guid id, string number, List<UpdateOrderOrderItemDto> orderItems)
        {
            Id = id;
            Number = number;
            OrderItems = orderItems;
        }

        public Guid Id { get; private set; }
        public string Number { get; set; }
        public List<UpdateOrderOrderItemDto> OrderItems { get; set; }

        public void SetId(Guid id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }
    }
}