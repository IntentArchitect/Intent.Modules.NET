using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Basics
{
    public class UpdateBasicCommand
    {
        public UpdateBasicCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Id { get; set; }

        public static UpdateBasicCommand Create(string name, string surname, Guid id)
        {
            return new UpdateBasicCommand
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }
    }
}