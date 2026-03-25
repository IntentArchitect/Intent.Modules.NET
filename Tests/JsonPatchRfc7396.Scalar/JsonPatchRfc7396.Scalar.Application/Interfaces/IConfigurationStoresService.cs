using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.ConfigurationStores;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Interfaces
{
    public interface IConfigurationStoresService
    {
        Task<Guid> CreateConfigurationStore(CreateConfigurationStoreDto dto, CancellationToken cancellationToken = default);
        Task UpdateConfigurationStore(Guid id, UpdateConfigurationStoreDto dto, CancellationToken cancellationToken = default);
        Task<ConfigurationStoreDto> PatchConfigurationStore(Guid id, PatchConfigurationStoreDto dto, CancellationToken cancellationToken = default);
        Task<ConfigurationStoreDto> FindConfigurationStoreById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ConfigurationStoreDto>> FindConfigurationStores(CancellationToken cancellationToken = default);
        Task DeleteConfigurationStore(Guid id, CancellationToken cancellationToken = default);
        Task<ConfigurationConfigurationItemDto> PatchConfigurationItem(Guid configurationStoreId, Guid id, PatchConfigurationItemDto dto, CancellationToken cancellationToken = default);
    }
}