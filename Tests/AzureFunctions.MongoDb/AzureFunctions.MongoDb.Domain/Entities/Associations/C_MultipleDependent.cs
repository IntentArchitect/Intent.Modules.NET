using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Associations
{
    public class C_MultipleDependent
    {
        private string? _id;

        public C_MultipleDependent()
        {
            Id = null!;
            Attribute = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Attribute { get; set; }
    }
}