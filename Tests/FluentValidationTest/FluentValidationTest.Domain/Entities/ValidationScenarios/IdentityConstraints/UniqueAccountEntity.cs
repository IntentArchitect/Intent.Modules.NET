using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints
{
    public class UniqueAccountEntity
    {
        public UniqueAccountEntity()
        {
            Username = null!;
            Email = null!;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}