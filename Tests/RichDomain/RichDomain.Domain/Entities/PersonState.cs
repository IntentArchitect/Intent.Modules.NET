using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;
using RichDomain.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public partial class Person : IPerson, IHasDomainEvent
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected Person()
        {
            FirstName = null!;
            CreatedBy = null!;
            Department = null!;
        }
        public Guid Id { get; private set; }

        public string FirstName { get; private set; }

        public Guid DepartmentId { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

        public string? UpdatedBy { get; private set; }

        public DateTimeOffset? UpdatedDate { get; private set; }

        public virtual Department Department { get; private set; }

        IDepartment IPerson.Department => Department;

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        void IPerson.UpdatePerson(string firstName, IDepartment department)
        {
            UpdatePerson(firstName, (Department)department);
        }

        void IAuditable.SetCreated(string createdBy, DateTimeOffset createdDate) => (CreatedBy, CreatedDate) = (createdBy, createdDate);

        void IAuditable.SetUpdated(string updatedBy, DateTimeOffset updatedDate) => (UpdatedBy, UpdatedDate) = (updatedBy, updatedDate);
    }
}