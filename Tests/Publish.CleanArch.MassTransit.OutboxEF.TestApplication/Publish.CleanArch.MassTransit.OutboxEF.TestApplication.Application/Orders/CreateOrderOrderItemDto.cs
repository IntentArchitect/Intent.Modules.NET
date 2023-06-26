using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders
{
    public class CreateOrderOrderItemDto
    {
        public CreateOrderOrderItemDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public decimal Amount { get; set; }

        public static CreateOrderOrderItemDto Create(string description, decimal amount)
        {
            return new CreateOrderOrderItemDto
            {
                Description = description,
                Amount = amount
            };
        }
    }
}