using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite
{
    public class PersistencePrecedence
    {
        public PersistencePrecedence()
        {
            RdbmsOnly = null!;
            DomainOnly = null!;
            BothDefined = null!;
        }

        public Guid Id { get; set; }

        public string RdbmsOnly { get; set; }

        public string DomainOnly { get; set; }

        public string BothDefined { get; set; }
    }
}