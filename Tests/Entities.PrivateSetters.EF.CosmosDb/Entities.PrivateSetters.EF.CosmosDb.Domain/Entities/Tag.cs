using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Domain.Entities
{
    public class Tag
    {
        public Tag(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Tag()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }
    }
}