using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Preferences
    {
        public Guid Id { get; set; }

        public bool Specials { get; set; }

        public bool News { get; set; }
    }
}