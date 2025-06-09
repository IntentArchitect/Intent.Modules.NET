using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class CompoundIndexEntityMultiChild
    {
        private string? _id;

        public CompoundIndexEntityMultiChild()
        {
            Id = null!;
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string CompoundOne { get; set; }

        public string CompoundTwo { get; set; }
    }
}