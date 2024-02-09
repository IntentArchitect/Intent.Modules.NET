using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
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