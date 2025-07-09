using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.DefaultDiagram
{
    public class ClassWithDefault : IHasDomainEvent
    {
        public ClassWithDefault(string param1 = "Constructor Param 1 Value")
        {
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ClassWithDefault()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = "John";

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void OperationWithDefault(string param1 = "Operation Param 1 Value")
        {
            // TODO: Implement OperationWithDefault (ClassWithDefault) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}