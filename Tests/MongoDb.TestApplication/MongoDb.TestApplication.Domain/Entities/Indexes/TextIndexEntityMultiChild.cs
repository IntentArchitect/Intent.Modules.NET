using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class TextIndexEntityMultiChild
    {
        private string? _id;

        public TextIndexEntityMultiChild()
        {
            Id = null!;
            FullText = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string FullText { get; set; }
    }
}