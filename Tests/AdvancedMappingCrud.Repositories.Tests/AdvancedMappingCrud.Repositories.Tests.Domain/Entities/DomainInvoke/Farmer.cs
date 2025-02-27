using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.DomainInvoke;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke
{
    public class Farmer : IHasDomainEvent
    {
        public Farmer(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Farmer()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public ICollection<Plot> Plots { get; set; } = [];

        public virtual ICollection<Machines> Machines { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void ChangeName(string name)
        {
            Name = name;
        }

        public void AddPlot(Plot address)
        {
            Plots.Add(address);
        }
    }
}