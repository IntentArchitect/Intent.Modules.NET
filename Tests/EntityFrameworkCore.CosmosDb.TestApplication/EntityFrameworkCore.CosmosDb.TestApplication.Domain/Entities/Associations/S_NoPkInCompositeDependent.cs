using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class S_NoPkInCompositeDependent
    {
        public S_NoPkInCompositeDependent()
        {
            Description = null!;
        }
        public string Description { get; set; }
    }
}