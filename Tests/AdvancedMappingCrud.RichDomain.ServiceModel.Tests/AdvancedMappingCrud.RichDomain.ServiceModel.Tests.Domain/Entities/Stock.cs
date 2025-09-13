using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class Stock : IHasDomainEvent
    {
        public Stock(string name, int total, string addedUser)
        {
            DomainEvents.Add(new StockCreatedEvent(name: name, total: total, addedUser: addedUser));
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
            DomainEvents.Add(new StockLevelUpdatedEvent(id: id, total: total, dateUpdated: dateUpdated));
        }
    }
}