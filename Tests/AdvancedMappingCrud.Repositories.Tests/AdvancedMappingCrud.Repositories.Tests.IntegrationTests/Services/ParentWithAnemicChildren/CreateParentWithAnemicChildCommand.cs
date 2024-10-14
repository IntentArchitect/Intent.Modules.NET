using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren
{
    public class CreateParentWithAnemicChildCommand
    {
        public CreateParentWithAnemicChildCommand()
        {
            Name = null!;
            Surname = null!;
            AnemicChildren = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public List<CreateParentWithAnemicChildAnemicChildDto> AnemicChildren { get; set; }

        public static CreateParentWithAnemicChildCommand Create(
            string name,
            string surname,
            List<CreateParentWithAnemicChildAnemicChildDto> anemicChildren)
        {
            return new CreateParentWithAnemicChildCommand
            {
                Name = name,
                Surname = surname,
                AnemicChildren = anemicChildren
            };
        }
    }
}