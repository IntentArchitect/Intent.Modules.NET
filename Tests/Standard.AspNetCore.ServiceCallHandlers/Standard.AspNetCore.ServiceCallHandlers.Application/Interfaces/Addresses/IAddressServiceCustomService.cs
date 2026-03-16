using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces.Addresses
{
    public interface IAddressServiceCustomService
    {
        Task<Guid> CreateAddressNoToken(CreateAddressDto dto);
        Guid UpdateAddressSync(Guid id, UpdateAddressDto dto);
    }
}