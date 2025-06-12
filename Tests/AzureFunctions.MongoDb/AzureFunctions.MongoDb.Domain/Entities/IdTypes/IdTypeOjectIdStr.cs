using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.IdTypes
{
    public class IdTypeOjectIdStr
    {
        public IdTypeOjectIdStr()
        {
            Id = null!;
            Attribute = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }
    }
}