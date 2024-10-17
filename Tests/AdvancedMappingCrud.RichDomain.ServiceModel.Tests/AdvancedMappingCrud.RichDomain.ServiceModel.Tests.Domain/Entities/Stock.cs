using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class Stock : IHasDomainEvent
    {
        public Stock(string name, int total, string addedUser)
        {
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

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void UpdateStockLevel(Guid id, int total, DateTime dateUpdated)
        {
            // [IntentFully]
            // TODO: Implement UpdateStockLevel (Stock) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}