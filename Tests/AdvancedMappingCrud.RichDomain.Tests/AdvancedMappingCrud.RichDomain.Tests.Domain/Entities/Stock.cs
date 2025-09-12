using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class Stock : IHasDomainEvent
    {
        public Stock(string name, int total, string addedUser)
        {
            DomainEvents.Add(new StockCreatedEvent(name: Name, total: Total, addedUser: AddedUser));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Stock()
        {
            Name = null!;
            AddedUser = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public int Total { get; private set; }

        public string AddedUser { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void UpdateStockLevel(Guid id, int total, DateTime dateUpdated)
        {
            DomainEvents.Add(new StockLevelUpdatedEvent(id: Id, total: Total, dateUpdated: DateUpdated));
        }
    }
}