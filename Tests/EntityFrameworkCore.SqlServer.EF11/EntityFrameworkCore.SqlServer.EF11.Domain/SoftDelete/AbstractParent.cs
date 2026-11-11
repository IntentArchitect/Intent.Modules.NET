using EntityFrameworkCore.SqlServer.EF11.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.SoftDelete
{
    public abstract class AbstractParent : ISoftDelete
    {
        public AbstractParent()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        void ISoftDelete.SetDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}