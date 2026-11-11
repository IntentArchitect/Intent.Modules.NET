using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.SoftDelete
{
    public class ChildOfTableParent : AbstractParentWithTable
    {
    }
}