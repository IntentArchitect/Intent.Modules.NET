using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SplitQueries.SqlServer.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime Created { get; set; }
    }
}