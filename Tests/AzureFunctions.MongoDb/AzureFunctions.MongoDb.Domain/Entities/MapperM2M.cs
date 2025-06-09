using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities
{
    public class MapperM2M
    {
        public MapperM2M()
        {
            Id = null!;
            Desc = null!;
        }

        public string Id { get; set; }

        public string Desc { get; set; }
    }
}