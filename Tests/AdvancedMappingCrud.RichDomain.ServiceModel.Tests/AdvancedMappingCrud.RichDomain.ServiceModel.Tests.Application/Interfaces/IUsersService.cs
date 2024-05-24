using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces
{
    public interface IUsersService : IDisposable
    {
        Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default);
        Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default);
        Task DeleteUser(Guid id, CancellationToken cancellationToken = default);
        Task<TestCompanyResultDto> TestEntity(Guid id, TestEntityDto dto, CancellationToken cancellationToken = default);
        Task<TestAddressDCResultDto?> TestDC(Guid id, TestDCDto dto, CancellationToken cancellationToken = default);
        Task<TestContactDetailsVOResultDto> TestVO(Guid id, TestVODto dto, CancellationToken cancellationToken = default);
        Task AddCollections(Guid id, AddCollectionsDto dto, CancellationToken cancellationToken = default);
    }
}