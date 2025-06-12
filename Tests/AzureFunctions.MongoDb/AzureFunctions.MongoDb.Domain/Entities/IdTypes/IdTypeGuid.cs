using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.IdTypes
{
    public class IdTypeGuid
    {
        public IdTypeGuid()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }
    }
}