using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Contracts;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.CreateUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(
                companyId: request.CompanyId,
                contactDetailsVO: new ContactDetailsVO(
                    cell: request.ContactDetailsVO.Cell,
                    email: request.ContactDetailsVO.Email),
                addresses: request.Addresses
                    .Select(a => new AddressDC(
                        line1: a.Line1,
                        line2: a.Line2,
                        city: a.City,
                        postal: a.Postal))
                    .ToList());

            _userRepository.Add(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}