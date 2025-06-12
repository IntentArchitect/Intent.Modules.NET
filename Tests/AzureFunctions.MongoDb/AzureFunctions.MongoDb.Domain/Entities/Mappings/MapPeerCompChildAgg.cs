using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapPeerCompChildAgg
    {
        public MapPeerCompChildAgg()
        {
            Id = null!;
            MapPeerCompChildAggAtt = null!;
        }

        public string Id { get; set; }

        public string MapPeerCompChildAggAtt { get; set; }
    }
}