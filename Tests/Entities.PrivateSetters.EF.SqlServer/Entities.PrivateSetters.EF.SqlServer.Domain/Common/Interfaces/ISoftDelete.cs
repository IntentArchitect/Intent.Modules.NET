using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.SoftDelete.SoftDeleteInterface", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Common.Interfaces
{
    public interface ISoftDelete : ISoftDeleteReadOnly
    {
        void SetDeleted(bool isDeleted);
    }

    public interface ISoftDeleteReadOnly
    {
        bool IsDeleted { get; }
    }
}