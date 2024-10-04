using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users.AddCollectionsUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AddCollectionsUserCommandHandler : IRequestHandler<AddCollectionsUserCommand>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public AddCollectionsUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AddCollectionsUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{request.Id}'");
            }

            user.AddCollections(request.Addresses
                .Select(a => new AddressDC(
                    line1: a.Line1,
                    line2: a.Line2,
                    city: a.City,
                    postal: a.Postal))
                .ToList(), request.Contacts
                .Select(c => new ContactDetailsVO(
                    cell: c.Cell,
                    email: c.Email))
                .ToList());
        }
    }
}