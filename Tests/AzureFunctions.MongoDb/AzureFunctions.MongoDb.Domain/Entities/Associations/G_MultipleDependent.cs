using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Associations
{
    public class G_MultipleDependent
    {
        private string? _id;

        public G_MultipleDependent()
        {
            Id = null!;
            Attribute = null!;
            G_RequiredCompositeNav = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Attribute { get; set; }

        public G_RequiredCompositeNav G_RequiredCompositeNav { get; set; }
    }
}