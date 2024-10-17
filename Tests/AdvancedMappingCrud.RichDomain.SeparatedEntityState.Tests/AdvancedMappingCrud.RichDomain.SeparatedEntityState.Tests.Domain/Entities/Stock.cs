using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Stock
    {
        public Stock(string name, int total, string addedUser)
        {
        }

        public void UpdateStockLevel(Guid id, int total, DateTime dateUpdated)
        {
            // [IntentFully]
            // TODO: Implement UpdateStockLevel (Stock) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}