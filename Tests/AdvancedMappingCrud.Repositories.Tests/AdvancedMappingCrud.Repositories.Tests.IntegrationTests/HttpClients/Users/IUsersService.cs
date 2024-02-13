using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Users
{
    public interface IUsersService : IDisposable
    {
        Task<Guid> CreateUserAddressAsync(CreateUserAddressCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default);
        Task DeleteUserAddressAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateUserAddressAsync(Guid id, UpdateUserAddressCommand command, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(Guid id, UpdateUserCommand command, CancellationToken cancellationToken = default);
        Task<UserAddressDto> GetUserAddressByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
        Task<List<UserAddressDto>> GetUserAddressesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<UserDto>> GetUsersAsync(string? name, string? surname, int pageNo, int pageSize, CancellationToken cancellationToken = default);
    }
}