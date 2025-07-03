using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients
{
    public class CreateClientCommand
    {
        public CreateClientCommand()
        {
            Name = null!;
            TagsIds = null!;
        }

        public string Name { get; set; }
        public List<string> TagsIds { get; set; }

        public static CreateClientCommand Create(string name, List<string> tagsIds)
        {
            return new CreateClientCommand
            {
                Name = name,
                TagsIds = tagsIds
            };
        }
    }
}