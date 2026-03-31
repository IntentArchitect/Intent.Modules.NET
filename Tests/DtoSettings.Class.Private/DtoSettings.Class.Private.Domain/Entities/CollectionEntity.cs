using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DtoSettings.Class.Private.Domain.Entities
{
    public class CollectionEntity
    {
        public CollectionEntity(Guid id, IEnumerable<string>? collection = null)
        {
            Id = id;
            Collection = new List<string>(collection ?? []);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected CollectionEntity()
        {
        }

        public Guid Id { get; set; }

        public IList<string> Collection { get; set; } = [];
    }
}