using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Entities
{
    public class Order
    {
        public Order()
        {
            ReferenceNumber = null!;
        }
        public Guid Id { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime Created { get; set; }
    }
}