using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces
{
    public interface IAddressesService
    {
        Task<Guid> CreateAddress();
        void UpdateAddress(Guid id);
        Task<AddressDto> FindAddressById(Guid id, CancellationToken cancellationToken = default);
        Task<List<AddressDto>> FindAddresses(CancellationToken cancellationToken = default);
        Task DeleteAddress(Guid id, CancellationToken cancellationToken = default);
    }
}