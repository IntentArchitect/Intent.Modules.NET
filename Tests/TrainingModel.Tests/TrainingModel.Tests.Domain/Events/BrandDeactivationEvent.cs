using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Common;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace TrainingModel.Tests.Domain.Events
{
    public class BrandDeactivationEvent : DomainEvent
    {
        public BrandDeactivationEvent(Brand brand)
        {
            Brand = brand;
        }

        public Brand Brand { get; }
    }
}