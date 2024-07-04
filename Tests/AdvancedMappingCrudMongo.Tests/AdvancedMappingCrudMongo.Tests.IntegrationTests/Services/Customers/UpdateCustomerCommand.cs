using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

        public static UpdateCustomerCommand Create(string name, string surname, string email, string id)
        {
            return new UpdateCustomerCommand
            {
                Name = name,
                Surname = surname,
                Email = email,
                Id = id
            };
        }
    }
}