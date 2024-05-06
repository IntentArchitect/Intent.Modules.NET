using System;
using Ardalis.GuardClauses;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Ardalis.Domain.Entities
{
    public class Client
    {
        public Client(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Client()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}