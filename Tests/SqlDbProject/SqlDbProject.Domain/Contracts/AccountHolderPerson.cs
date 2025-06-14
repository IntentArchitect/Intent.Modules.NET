using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace SqlDbProject.Domain.Contracts
{
    public record AccountHolderPerson
    {
        public AccountHolderPerson(long stakeholderId, DateTime birthdate, int sexTypeId, string description, decimal height)
        {
            StakeholderId = stakeholderId;
            Birthdate = birthdate;
            SexTypeId = sexTypeId;
            Description = description;
            Height = height;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected AccountHolderPerson()
        {
            Description = null!;
        }

        public long StakeholderId { get; init; }
        public DateTime Birthdate { get; init; }
        public int SexTypeId { get; init; }
        public string Description { get; init; }
        public decimal Height { get; init; }
    }
}