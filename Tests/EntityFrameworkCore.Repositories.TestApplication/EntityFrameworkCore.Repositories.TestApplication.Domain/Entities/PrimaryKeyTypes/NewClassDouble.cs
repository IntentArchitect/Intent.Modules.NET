using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes
{
    public class NewClassDouble : IHasDomainEvent
    {
        public NewClassDouble()
        {
            DoubleName = null!;
        }
        public double Id { get; set; }

        public string DoubleName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}