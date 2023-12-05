using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities
{
    public class Preference
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public Guid UserId { get; set; }
    }
}