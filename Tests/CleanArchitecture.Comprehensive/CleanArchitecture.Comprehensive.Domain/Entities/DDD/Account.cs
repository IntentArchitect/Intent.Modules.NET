using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.DDD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Account
    {

        public Guid Id { get; set; }

        public string AccNumber { get; set; }

        public Guid AccountHolderId { get; set; }

        public string Note { get; set; }

        public string UpdateNote(string note)
        {
            Note = note;
            return note;
        }
    }
}