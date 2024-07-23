using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies
{
    public class UpdateBasicOrderByCommand
    {
        public UpdateBasicOrderByCommand()
        {
            Name = null!;
            Surname = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Id { get; set; }

        public static UpdateBasicOrderByCommand Create(string name, string surname, string id)
        {
            return new UpdateBasicOrderByCommand
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }
    }
}