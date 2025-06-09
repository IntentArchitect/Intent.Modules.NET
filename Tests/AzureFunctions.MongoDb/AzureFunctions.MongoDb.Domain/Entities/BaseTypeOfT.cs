using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities
{
    public abstract class BaseTypeOfT<T>
    {
        public BaseTypeOfT()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public T BaseAttribute { get; set; }
    }
}