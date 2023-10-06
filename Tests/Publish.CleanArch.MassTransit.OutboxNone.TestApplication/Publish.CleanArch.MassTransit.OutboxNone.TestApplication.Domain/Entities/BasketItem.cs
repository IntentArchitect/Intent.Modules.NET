using System;
using Intent.RoslynWeaver.Attributes;

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities
{
    public class BasketItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public Guid BasketId { get; set; }
    }
}