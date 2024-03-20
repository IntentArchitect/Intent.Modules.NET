using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.DomainServices
{
    public class BaseEntityB : IHasDomainEvent
    {
        public BaseEntityB(string baseAttr)
        {
            BaseAttr = baseAttr;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected BaseEntityB()
        {
            BaseAttr = null!;
        }

        public Guid Id { get; set; }

        public string BaseAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}