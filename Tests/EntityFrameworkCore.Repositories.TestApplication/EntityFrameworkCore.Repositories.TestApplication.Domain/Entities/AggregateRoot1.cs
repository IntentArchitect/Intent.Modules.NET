using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    public class AggregateRoot1 : IHasDomainEvent
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Here is a multi
        /// line comment that
        /// is supposed to work for "HasComment()"
        /// and has some quotes included
        /// </summary>
        public string Tag { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public AggregateRoot1 Operation(object param1)
        {
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}