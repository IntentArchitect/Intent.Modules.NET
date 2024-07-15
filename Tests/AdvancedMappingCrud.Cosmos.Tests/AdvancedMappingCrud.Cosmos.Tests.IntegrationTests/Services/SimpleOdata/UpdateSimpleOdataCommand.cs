using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata
{
    public class UpdateSimpleOdataCommand
    {
        public UpdateSimpleOdataCommand()
        {
            Name = null!;
            Surname = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Id { get; set; }

        public static UpdateSimpleOdataCommand Create(string name, string surname, string id)
        {
            return new UpdateSimpleOdataCommand
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }
    }
}