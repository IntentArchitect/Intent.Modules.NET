using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints
{
    public class UniquePersonEntity
    {
        public UniquePersonEntity()
        {
            FirstName = null!;
            LastName = null!;
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? ContactNumber { get; set; }
    }
}