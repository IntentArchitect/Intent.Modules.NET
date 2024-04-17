using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities
{
    public class MapperM2M
    {
        public string Id { get; set; }

        public string Desc { get; set; }
    }
}