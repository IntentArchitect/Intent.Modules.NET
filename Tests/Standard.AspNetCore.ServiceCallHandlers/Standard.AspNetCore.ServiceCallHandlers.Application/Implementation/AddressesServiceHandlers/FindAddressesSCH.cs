using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class FindAddressesSCH
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public FindAddressesSCH(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AddressDto>> Handle(CancellationToken cancellationToken = default)
        {
            var addresses = await _addressRepository.FindAllAsync(cancellationToken);
            return addresses.MapToAddressDtoList(_mapper);
        }
    }
}