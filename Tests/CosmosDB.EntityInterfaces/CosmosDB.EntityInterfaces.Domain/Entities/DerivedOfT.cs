using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>, IDerivedOfT
    {
        public string DerivedAttribute { get; set; }
    }
}