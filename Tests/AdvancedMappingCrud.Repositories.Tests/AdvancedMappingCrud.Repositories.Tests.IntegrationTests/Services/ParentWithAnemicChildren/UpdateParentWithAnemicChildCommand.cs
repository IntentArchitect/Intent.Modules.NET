using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren
{
    public class UpdateParentWithAnemicChildCommand
    {
        public UpdateParentWithAnemicChildCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Id { get; set; }

        public static UpdateParentWithAnemicChildCommand Create(string name, string surname, Guid id)
        {
            return new UpdateParentWithAnemicChildCommand
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }
    }
}