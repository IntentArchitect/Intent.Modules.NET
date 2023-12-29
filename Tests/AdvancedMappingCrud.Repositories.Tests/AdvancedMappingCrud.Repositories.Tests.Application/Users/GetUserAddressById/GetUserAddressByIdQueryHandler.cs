using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUserAddressById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUserAddressByIdQueryHandler : IRequestHandler<GetUserAddressByIdQuery, UserAddressDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetUserAddressByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserAddressDto> Handle(GetUserAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find UserAddress '{request.UserId}'");
            }

            var address = user.Addresses.FirstOrDefault(x => x.Id == request.Id && x.UserId == request.UserId);
            if (address is null)
            {
                throw new NotFoundException($"Could not find UserAddress '({request.Id}, {request.UserId})'");
            }
            return address.MapToUserAddressDto(_mapper);
        }
    }
}