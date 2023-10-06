using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public abstract class MiddleAbstract_Middle : MiddleAbstract_Root
    {

        public string MiddleAttribute { get; set; }
    }
}