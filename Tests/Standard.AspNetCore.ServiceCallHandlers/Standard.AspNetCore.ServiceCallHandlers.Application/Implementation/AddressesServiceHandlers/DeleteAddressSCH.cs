using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Common.Exceptions;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class DeleteAddressSCH
    {
        private readonly IAddressRepository _addressRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteAddressSCH(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Guid id, CancellationToken cancellationToken = default)
        {
            var address = await _addressRepository.FindByIdAsync(id, cancellationToken);
            if (address is null)
            {
                throw new NotFoundException($"Could not find Address '{id}'");
            }


            _addressRepository.Remove(address);
        }
    }
}