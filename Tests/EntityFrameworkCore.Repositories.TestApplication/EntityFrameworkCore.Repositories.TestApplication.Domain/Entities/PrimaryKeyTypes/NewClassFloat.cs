using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes
{
    public class NewClassFloat : IHasDomainEvent
    {
        public NewClassFloat()
        {
            FloatName = null!;
        }
        public float Id { get; set; }

        public string FloatName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}