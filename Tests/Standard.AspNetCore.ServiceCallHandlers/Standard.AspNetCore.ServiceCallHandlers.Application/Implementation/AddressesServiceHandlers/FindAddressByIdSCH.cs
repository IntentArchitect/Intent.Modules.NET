using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Common.Exceptions;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class FindAddressByIdSCH
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public FindAddressByIdSCH(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AddressDto> Handle(Guid id, CancellationToken cancellationToken = default)
        {
            var address = await _addressRepository.FindByIdAsync(id, cancellationToken);
            if (address is null)
            {
                throw new NotFoundException($"Could not find Address '{id}'");
            }
            return address.MapToAddressDto(_mapper);
        }
    }
}