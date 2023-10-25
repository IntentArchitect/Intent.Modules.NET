using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>
    {
        public string DerivedAttribute { get; private set; }
    }
}