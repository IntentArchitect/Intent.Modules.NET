using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    public class WeirdClass : Composite
    {
        public WeirdClass()
        {
            WeirdField = null!;
        }

        public string WeirdField { get; set; }
    }
}