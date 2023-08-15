using System;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public interface IPerson : IHasDomainEvent
    {
        Guid Id { get; }

        string FirstName { get; }

        Guid DepartmentId { get; }

        IDepartment Department { get; }

        void UpdatePerson(string firstName);

        void UpdatePerson(string firstName, IDepartment department);
    }
}