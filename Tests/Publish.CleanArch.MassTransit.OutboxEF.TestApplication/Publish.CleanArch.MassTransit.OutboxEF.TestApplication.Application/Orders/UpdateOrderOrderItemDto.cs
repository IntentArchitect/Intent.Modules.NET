using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders
{
    public class UpdateOrderOrderItemDto
    {
        public UpdateOrderOrderItemDto()
        {
            Description = null!;
        }

        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public Guid Id { get; set; }

        public static UpdateOrderOrderItemDto Create(Guid orderId, string description, decimal amount, Guid id)
        {
            return new UpdateOrderOrderItemDto
            {
                OrderId = orderId,
                Description = description,
                Amount = amount,
                Id = id
            };
        }
    }
}