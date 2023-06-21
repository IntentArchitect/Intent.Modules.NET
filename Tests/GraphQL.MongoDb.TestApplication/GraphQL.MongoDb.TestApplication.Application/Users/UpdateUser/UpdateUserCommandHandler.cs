using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Common.Exceptions;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.UpdateUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException($"Could not find User {request.Id}");
            }
            existingUser.Name = request.Name;
            existingUser.Surname = request.Surname;
            existingUser.Email = request.Email;

            _userRepository.Update(existingUser);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return existingUser.MapToUserDto(_mapper);
        }
    }
}