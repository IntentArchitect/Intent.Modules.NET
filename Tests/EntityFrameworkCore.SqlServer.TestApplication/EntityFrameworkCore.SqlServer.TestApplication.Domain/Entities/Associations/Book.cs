using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    public class Book : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}