using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AddressesService : IAddressesService
    {
        private readonly IServiceProvider _serviceProvider;

        [IntentManaged(Mode.Merge)]
        public AddressesService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateAddress()
        {
            var sch = (CreateAddressSCH)_serviceProvider.GetRequiredService(typeof(CreateAddressSCH));
            var result = await sch.Handle();
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public void UpdateAddress(Guid id)
        {
            var sch = (UpdateAddressSCH)_serviceProvider.GetRequiredService(typeof(UpdateAddressSCH));
            sch.Handle(id);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AddressDto> FindAddressById(Guid id, CancellationToken cancellationToken = default)
        {
            var sch = (FindAddressByIdSCH)_serviceProvider.GetRequiredService(typeof(FindAddressByIdSCH));
            var result = await sch.Handle(id, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AddressDto>> FindAddresses(CancellationToken cancellationToken = default)
        {
            var sch = (FindAddressesSCH)_serviceProvider.GetRequiredService(typeof(FindAddressesSCH));
            var result = await sch.Handle(cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteAddress(Guid id, CancellationToken cancellationToken = default)
        {
            var sch = (DeleteAddressSCH)_serviceProvider.GetRequiredService(typeof(DeleteAddressSCH));
            await sch.Handle(id, cancellationToken);
        }
    }
}