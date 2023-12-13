using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities.Folder
{
    public class FolderContainer : IHasDomainEvent
    {
        private string? _id;
        private string? _folderPartitionKey;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string FolderPartitionKey
        {
            get => _folderPartitionKey ??= Guid.NewGuid().ToString();
            private set => _folderPartitionKey = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}