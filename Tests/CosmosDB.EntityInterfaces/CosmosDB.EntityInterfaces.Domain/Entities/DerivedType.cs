using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class DerivedType : BaseType, IDerivedType
    {
        public string DerivedTypeAggregateId { get; set; }
    }
}