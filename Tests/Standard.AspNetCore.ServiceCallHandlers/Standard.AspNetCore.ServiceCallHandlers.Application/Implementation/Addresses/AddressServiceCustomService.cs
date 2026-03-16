using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressServiceCustomHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces.Addresses;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.Addresses
{
    [IntentManaged(Mode.Merge)]
    public class AddressServiceCustomService : IAddressServiceCustomService
    {
        private readonly IServiceProvider _serviceProvider;

        [IntentManaged(Mode.Merge)]
        public AddressServiceCustomService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateAddressNoToken(CreateAddressDto dto)
        {
            var sch = (CreateAddressNoTokenSCH)_serviceProvider.GetRequiredService(typeof(CreateAddressNoTokenSCH));
            var result = await sch.Handle(dto);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public Guid UpdateAddressSync(Guid id, UpdateAddressDto dto)
        {
            var sch = (UpdateAddressSyncSCH)_serviceProvider.GetRequiredService(typeof(UpdateAddressSyncSCH));
            var result = sch.Handle(id, dto);
            return result;
        }
    }
}