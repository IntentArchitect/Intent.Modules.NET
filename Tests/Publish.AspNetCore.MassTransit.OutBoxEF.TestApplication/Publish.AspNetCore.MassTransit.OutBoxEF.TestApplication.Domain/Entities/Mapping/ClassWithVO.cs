using System;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Mapping;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities.Mapping
{
    public class ClassWithVO
    {
        public Guid Id { get; set; }

        public TestVO TestVO { get; set; }
    }
}