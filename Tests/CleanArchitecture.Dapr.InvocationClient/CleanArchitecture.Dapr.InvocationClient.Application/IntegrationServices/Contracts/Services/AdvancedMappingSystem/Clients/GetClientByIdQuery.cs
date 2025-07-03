using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients
{
    public class GetClientByIdQuery
    {
        public GetClientByIdQuery()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static GetClientByIdQuery Create(string id)
        {
            return new GetClientByIdQuery
            {
                Id = id
            };
        }
    }
}