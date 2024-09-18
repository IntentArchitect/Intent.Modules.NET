using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EnumStrings.Domain.Entities
{
    public class RootEntity
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public EnumExample EnumExample { get; set; }

        public EnumExample? NullableEnumExample { get; set; }

        public EmbeddedObject Embedded { get; set; }

        public ICollection<NestedEntity> NestedEntities { get; set; } = new List<NestedEntity>();
    }
}