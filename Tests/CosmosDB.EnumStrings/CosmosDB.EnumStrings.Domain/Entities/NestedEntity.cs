using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EnumStrings.Domain.Entities
{
    public class NestedEntity
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

        public EmbeddedObject EmbeddedObject2 { get; set; }

        public EmbeddedObject EmbeddedObject { get; set; }
    }
}