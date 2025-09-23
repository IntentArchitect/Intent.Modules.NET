using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Stock
    {
        public Stock(string name, int total, string addedUser)
        {
            DomainEvents.Add(new StockCreatedEvent(
                name: name,
                total: total,
                addedUser: addedUser));
        }

        public void UpdateStockLevel(Guid id, int total, DateTime dateUpdated)
        {
            DomainEvents.Add(new StockLevelUpdatedEvent(
                id: id,
                total: total,
                dateUpdated: dateUpdated));
        }
    }
}