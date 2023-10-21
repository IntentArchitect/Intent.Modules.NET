using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.Dapr.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>
    {
        public string DerivedAttribute { get; set; }
    }
}