using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public abstract class MiddleAbstract_Middle : MiddleAbstract_Root
    {

        public string MiddleAttribute { get; set; }
    }
}