using System;
using System.Collections.Generic;
using CleanArchitecture.Dapr.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Dapr.Domain.Entities
{
    public class Country
    {
        private int? _id;
        public int Id
        {
            get => _id ?? throw new NullReferenceException("_id has not been set");
            set => _id = value;
        }

        public string Name { get; set; }
    }
}