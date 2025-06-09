using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Associations
{
    public class A_RequiredComposite
    {
        public A_RequiredComposite()
        {
            Id = null!;
            ReqCompAttribute = null!;
        }

        public string Id { get; set; }

        public string ReqCompAttribute { get; set; }

        public A_OptionalDependent? A_OptionalDependent { get; set; }
    }
}