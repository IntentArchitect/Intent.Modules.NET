using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class CompoundIndexEntitySingleChild
    {
        public CompoundIndexEntitySingleChild()
        {
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string CompoundOne { get; set; }

        public string CompoundTwo { get; set; }
    }
}