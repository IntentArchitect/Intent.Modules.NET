using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class CompoundIndexEntity
    {
        public CompoundIndexEntity()
        {
            Id = null!;
            SomeField = null!;
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public string CompoundOne { get; set; }

        public string CompoundTwo { get; set; }
    }
}