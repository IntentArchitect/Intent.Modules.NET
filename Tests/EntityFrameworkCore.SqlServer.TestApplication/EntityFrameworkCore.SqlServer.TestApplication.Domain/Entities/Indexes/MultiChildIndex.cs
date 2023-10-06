using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes
{
    public class MultiChildIndex
    {
        public Guid Id { get; set; }

        public Guid ParentIndexId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}