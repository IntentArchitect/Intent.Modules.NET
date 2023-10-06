using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public class MiddleAbstract_Leaf : MiddleAbstract_Middle
    {

        public string LeafAttribute { get; set; }
    }
}