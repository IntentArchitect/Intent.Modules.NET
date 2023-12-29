using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.UpdateUserAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUserAddressCommandHandler : IRequestHandler<UpdateUserAddressCommand>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateUserAddressCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find UserAddress '{request.UserId}'");
            }

            var address = user.Addresses.FirstOrDefault(x => x.Id == request.Id);
            if (address is null)
            {
                throw new NotFoundException($"Could not find UserAddress '{request.Id}'");
            }

            address.UserId = request.UserId;
            address.Line1 = request.Line1;
            address.Line2 = request.Line2;
            address.City = request.City;
            address.Postal = request.Postal;
        }
    }
}