using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapCompChildAgg
    {
        public MapCompChildAgg()
        {
            Id = null!;
            CompChildAggAtt = null!;
        }

        public string Id { get; set; }

        public string CompChildAggAtt { get; set; }
    }
}