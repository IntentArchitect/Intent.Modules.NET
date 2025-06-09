using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapMapMe
    {
        public MapMapMe()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}