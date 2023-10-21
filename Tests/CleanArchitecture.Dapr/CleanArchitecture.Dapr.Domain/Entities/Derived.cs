using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.Dapr.Domain.Entities
{
    public class Derived : BaseType
    {
        public string Attribute { get; set; }
    }
}