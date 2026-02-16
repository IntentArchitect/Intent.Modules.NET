using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Interfaces
{
    public interface IClientsTraditionalService
    {
        Task CallGetClientsQuery(CancellationToken cancellationToken = default);
        Task CallGetClientByIdQuery(string id, CancellationToken cancellationToken = default);
        Task CallCreateClientCommand(string name, List<string> tagsIds, CancellationToken cancellationToken = default);
        Task CallDeleteClientCommand(string id, CancellationToken cancellationToken = default);
        Task CallUpdateClientCommand(string id, string name, List<string> tagsIds, CancellationToken cancellationToken = default);
        Task CallGetClientExtraFieldsQuery(Guid id, string field1, string field2, CancellationToken cancellationToken = default);
    }
}