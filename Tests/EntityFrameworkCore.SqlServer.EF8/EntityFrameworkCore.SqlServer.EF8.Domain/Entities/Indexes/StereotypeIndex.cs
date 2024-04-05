using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Indexes
{
    public class StereotypeIndex
    {
        public Guid Id { get; set; }

        public Guid DefaultIndexField { get; set; }

        public Guid CustomIndexField { get; set; }

        public Guid GroupedIndexFieldA { get; set; }

        public Guid GroupedIndexFieldB { get; set; }
    }
}