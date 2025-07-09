using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    public class MapAggChild
    {
        public MapAggChild()
        {
            Id = null!;
            ChildName = null!;
        }

        public string Id { get; set; }

        public string ChildName { get; set; }
    }
}