using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers
{
    public class UpdateFarmerCommand
    {
        public UpdateFarmerCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Id { get; set; }

        public static UpdateFarmerCommand Create(string name, string surname, Guid id)
        {
            return new UpdateFarmerCommand
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }
    }
}