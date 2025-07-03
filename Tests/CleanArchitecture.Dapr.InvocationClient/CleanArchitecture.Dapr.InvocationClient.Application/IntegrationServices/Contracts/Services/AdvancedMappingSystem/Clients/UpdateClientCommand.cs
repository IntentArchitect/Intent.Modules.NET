using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients
{
    public class UpdateClientCommand
    {
        public UpdateClientCommand()
        {
            Id = null!;
            Name = null!;
            TagsIds = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> TagsIds { get; set; }

        public static UpdateClientCommand Create(string id, string name, List<string> tagsIds)
        {
            return new UpdateClientCommand
            {
                Id = id,
                Name = name,
                TagsIds = tagsIds
            };
        }
    }
}