using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.CreateUserAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUserAddressCommandHandler : IRequestHandler<CreateUserAddressCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public CreateUserAddressCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find UserAddress '{request.UserId}'");
            }
            var address = new UserAddress
            {
                UserId = request.UserId,
                Line1 = request.Line1,
                Line2 = request.Line2,
                City = request.City,
                Postal = request.Postal
            };

            user.Addresses.Add(address);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return address.Id;
        }
    }
}