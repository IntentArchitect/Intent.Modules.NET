using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class SingleIndexEntityMultiChild
    {
        private string? _id;

        public SingleIndexEntityMultiChild()
        {
            Id = null!;
            SingleIndex = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string SingleIndex { get; set; }
    }
}