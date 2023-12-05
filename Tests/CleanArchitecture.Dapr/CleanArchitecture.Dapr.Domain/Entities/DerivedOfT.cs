using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Dapr.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>
    {
        public string DerivedAttribute { get; set; }
    }
}