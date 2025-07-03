using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients
{
    public class DeleteClientCommand
    {
        public DeleteClientCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteClientCommand Create(string id)
        {
            return new DeleteClientCommand
            {
                Id = id
            };
        }
    }
}