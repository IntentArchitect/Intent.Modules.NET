using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Price
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public DateTime ActiveFrom { get; set; }

        public decimal Amount { get; set; }
    }
}