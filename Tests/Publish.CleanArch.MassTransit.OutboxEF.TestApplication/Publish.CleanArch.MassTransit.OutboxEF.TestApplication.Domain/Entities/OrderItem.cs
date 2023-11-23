using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}