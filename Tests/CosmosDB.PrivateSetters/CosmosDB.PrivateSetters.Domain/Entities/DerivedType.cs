using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class DerivedType : BaseType
    {
        public string DerivedTypeAggregateId { get; private set; }
    }
}