using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren
{
    public class ParentWithAnemicChildDto
    {
        public ParentWithAnemicChildDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static ParentWithAnemicChildDto Create(Guid id, string name, string surname)
        {
            return new ParentWithAnemicChildDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }
    }
}