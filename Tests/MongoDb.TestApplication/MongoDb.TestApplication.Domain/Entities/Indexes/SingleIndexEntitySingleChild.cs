using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class SingleIndexEntitySingleChild
    {
        public SingleIndexEntitySingleChild()
        {
            SingleIndex = null!;
        }

        public string SingleIndex { get; set; }
    }
}