using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public partial class Person : IPerson, IHasDomainEvent
    {
        public Guid Id { get; private set; }

        public string FirstName { get; private set; }

        public Guid DepartmentId { get; private set; }

        public virtual Department Department { get; private set; }

        IDepartment IPerson.Department => Department;

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        void IPerson.UpdatePerson(string firstName, IDepartment department)
        {
            UpdatePerson(firstName, (Department)department);
        }
    }
}