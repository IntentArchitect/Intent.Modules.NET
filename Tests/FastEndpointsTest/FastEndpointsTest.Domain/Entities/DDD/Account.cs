using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.DDD
{
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