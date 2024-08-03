using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.SoftDelete.SoftDeleteInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Common.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}