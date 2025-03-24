using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace Solace.Tests.Domain.Contracts
{
    public record CustomerCustom
    {
        public CustomerCustom(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected CustomerCustom()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; init; }
        public string Surname { get; init; }
    }
}