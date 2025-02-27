using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Farmers
{
    public interface IFarmersService : IDisposable
    {
        Task AddPlotFarmerAsync(Guid id, AddPlotFarmerCommand command, CancellationToken cancellationToken = default);
        Task ChangeNameFarmerAsync(Guid id, ChangeNameFarmerCommand command, CancellationToken cancellationToken = default);
        Task ChangeNameMachinesAsync(Guid farmerId, Guid id, ChangeNameMachinesCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateFarmerAsync(CreateFarmerCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateMachinesAsync(Guid farmerId, CreateMachinesCommand command, CancellationToken cancellationToken = default);
        Task DeleteFarmerAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteMachinesAsync(Guid farmerId, Guid id, CancellationToken cancellationToken = default);
        Task UpdateFarmerAsync(Guid id, UpdateFarmerCommand command, CancellationToken cancellationToken = default);
        Task UpdateMachinesAsync(Guid farmerId, Guid id, UpdateMachinesCommand command, CancellationToken cancellationToken = default);
        Task<FarmerDto> GetFarmerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<FarmerDto>> GetFarmersAsync(CancellationToken cancellationToken = default);
        Task<MachinesDto> GetMachinesByIdAsync(Guid farmerId, Guid id, CancellationToken cancellationToken = default);
        Task<List<MachinesDto>> GetMachinesAsync(Guid farmerId, CancellationToken cancellationToken = default);
    }
}