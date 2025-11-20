using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.Indexes
{
    public class SortDirectionStereotype
    {
        public SortDirectionStereotype()
        {
            FieldA = null!;
            FieldB = null!;
        }
        public Guid Id { get; set; }

        public string FieldA { get; set; }

        public string FieldB { get; set; }
    }
}