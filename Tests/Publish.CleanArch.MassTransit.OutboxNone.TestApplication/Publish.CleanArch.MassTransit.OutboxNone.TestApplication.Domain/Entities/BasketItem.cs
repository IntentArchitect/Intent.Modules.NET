using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

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