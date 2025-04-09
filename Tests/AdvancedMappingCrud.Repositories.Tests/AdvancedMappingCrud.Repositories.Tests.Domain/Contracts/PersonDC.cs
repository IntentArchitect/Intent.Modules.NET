using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Contracts
{
    public record PersonDC
    {
        public PersonDC(string firstName, string surname)
        {
            FirstName = firstName;
            Surname = surname;
        }
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected PersonDC()
        {
            FirstName = null!;
            Surname = null!;
        }

        public string FirstName { get; init; }
        public string Surname { get; init; }
    }
}