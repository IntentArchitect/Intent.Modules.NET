using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>
    {
        public string DerivedAttribute { get; set; }
    }
}