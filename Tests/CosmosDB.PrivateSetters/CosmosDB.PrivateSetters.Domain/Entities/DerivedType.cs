using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class DerivedType : BaseType
    {
        public string DerivedTypeAggregateId { get; private set; }
    }
}