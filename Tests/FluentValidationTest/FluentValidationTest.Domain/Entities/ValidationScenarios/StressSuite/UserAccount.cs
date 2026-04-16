using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite
{
    public class UserAccount
    {
        public UserAccount()
        {
            Email = null!;
        }

        public Guid Id { get; set; }

        public string Email { get; set; }
    }
}