using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.Domain.Entities
{
    public class DerivedType : BaseType
    {
        public string DerivedTypeAggregateId { get; set; }
    }
}