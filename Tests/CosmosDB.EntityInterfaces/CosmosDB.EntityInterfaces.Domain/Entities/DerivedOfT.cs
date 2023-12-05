using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>, IDerivedOfT
    {
        public string DerivedAttribute { get; set; }
    }
}