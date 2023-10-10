using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.DDD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class DataContractClass : IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Change(DataContractObject obj)
        {

        }
    }
}