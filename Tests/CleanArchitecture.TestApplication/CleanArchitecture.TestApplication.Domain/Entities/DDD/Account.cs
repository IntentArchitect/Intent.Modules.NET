using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.DDD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Account
    {
        public Account()
        {
            AccNumber = null!;
            Note = null!;
        }

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