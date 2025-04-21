using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace SqlDbProject.Domain.Contracts
{
    public record ShareholderPerson
    {
        public ShareholderPerson(long stakeholderId, DateTime birthdate, int sexTypeId, string description)
        {
            StakeholderId = stakeholderId;
            Birthdate = birthdate;
            SexTypeId = sexTypeId;
            Description = description;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected ShareholderPerson()
        {
            Description = null!;
        }

        public long StakeholderId { get; init; }
        public DateTime Birthdate { get; init; }
        public int SexTypeId { get; init; }
        public string Description { get; init; }
    }
}