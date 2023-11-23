using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_MiddleAbstract_Leaf : TPH_MiddleAbstract_Middle
    {

        public string LeafAttribute { get; set; }
    }
}