using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Common;
using TrainingModel.Tests.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Brand : IHasDomainEvent
    {
        public Brand(string name)
        {
            Name = name;
            IsActive = true;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Brand()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Deactivate()
        {
            if (!IsActive) return;
            DomainEvents.Add(new BrandDeactivationEvent(brand: this));
            IsActive = false;
        }
    }
}