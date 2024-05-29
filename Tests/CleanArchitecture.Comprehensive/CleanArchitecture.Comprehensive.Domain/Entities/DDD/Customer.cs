using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.DDD
{
    public class Customer : Person
    {

        public string Email { get; set; }

        public Address Address { get; set; }
    }
}