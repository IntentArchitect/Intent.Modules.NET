using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class UpdateParentCommand
    {
        public UpdateParentCommand()
        {
            Name = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public List<UpdateParentCommandChildrenDto>? Children { get; set; }
        public UpdateParentCommandParentDetailsDto? ParentDetails { get; set; }

        public static UpdateParentCommand Create(
            string name,
            string id,
            List<UpdateParentCommandChildrenDto>? children,
            UpdateParentCommandParentDetailsDto? parentDetails)
        {
            return new UpdateParentCommand
            {
                Name = name,
                Id = id,
                Children = children,
                ParentDetails = parentDetails
            };
        }
    }
}