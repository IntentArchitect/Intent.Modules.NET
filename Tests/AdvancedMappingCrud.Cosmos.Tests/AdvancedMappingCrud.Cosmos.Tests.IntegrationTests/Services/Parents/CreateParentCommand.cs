using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class CreateParentCommand
    {
        public CreateParentCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public List<CreateParentCommandChildrenDto>? Children { get; set; }
        public CreateParentCommandParentDetailsDto? ParentDetails { get; set; }

        public static CreateParentCommand Create(
            string name,
            List<CreateParentCommandChildrenDto>? children,
            CreateParentCommandParentDetailsDto? parentDetails)
        {
            return new CreateParentCommand
            {
                Name = name,
                Children = children,
                ParentDetails = parentDetails
            };
        }
    }
}