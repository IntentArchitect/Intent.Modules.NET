using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Common.Exceptions;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.DeleteUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public DeleteUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException($"Could not find User '{request.Id}' ");
            }
            _userRepository.Remove(existingUser);
            return existingUser.MapToUserDto(_mapper);
        }
    }
}