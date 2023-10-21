using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations
{
    public abstract class TPH_MiddleAbstract_Middle : TPH_MiddleAbstract_Root
    {

        public string MiddleAttribute { get; set; }
    }
}