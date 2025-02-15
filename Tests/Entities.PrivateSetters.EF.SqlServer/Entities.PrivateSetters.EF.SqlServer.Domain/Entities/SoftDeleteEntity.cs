using System;
using Entities.PrivateSetters.EF.SqlServer.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    public class SoftDeleteEntity : ISoftDelete
    {
        public SoftDeleteEntity()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public bool IsDeleted { get; private set; }

        void ISoftDelete.SetDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}