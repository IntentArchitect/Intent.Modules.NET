using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RichDomain.Domain.Contracts.Inheritance
{
    public record SuperContract : BaseContract
    {
        public SuperContract(int id, DateTime created, string name) : base(id, created)
        {
            Name = name;
        }

        public string Name { get; init; }
    }
}