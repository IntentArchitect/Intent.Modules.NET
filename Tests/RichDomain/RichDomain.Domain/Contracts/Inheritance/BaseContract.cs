using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RichDomain.Domain.Contracts.Inheritance
{
    public record BaseContract
    {
        public BaseContract(int id, DateTime created)
        {
            Id = id;
            Created = created;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected BaseContract()
        {
        }

        public int Id { get; init; }
        public DateTime Created { get; init; }
    }
}