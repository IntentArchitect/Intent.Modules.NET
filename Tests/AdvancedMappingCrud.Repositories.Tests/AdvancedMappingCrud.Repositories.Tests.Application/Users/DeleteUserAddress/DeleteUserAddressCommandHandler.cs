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

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.DeleteUserAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteUserAddressCommandHandler : IRequestHandler<DeleteUserAddressCommand>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteUserAddressCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
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

            user.Addresses.Remove(address);
        }
    }
}