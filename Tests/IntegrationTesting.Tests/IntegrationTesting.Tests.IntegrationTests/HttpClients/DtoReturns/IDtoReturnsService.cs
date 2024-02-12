using IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.DtoReturns
{
    public interface IDtoReturnsService : IDisposable
    {
        Task<DtoReturnDto> CreateDtoReturnAsync(CreateDtoReturnCommand command, CancellationToken cancellationToken = default);
        Task<DtoReturnDto> DeleteDtoReturnAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DtoReturnDto> UpdateDtoReturnAsync(Guid id, UpdateDtoReturnCommand command, CancellationToken cancellationToken = default);
        Task<DtoReturnDto> GetDtoReturnByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<DtoReturnDto>> GetDtoReturnsAsync(CancellationToken cancellationToken = default);
    }
}