using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestVOUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestVOUserCommandHandler : IRequestHandler<TestVOUserCommand, TestContactDetailsVODto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public TestVOUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestContactDetailsVODto> Handle(TestVOUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{request.Id}'");
            }

            var testVOResult = user.TestVO();
            return testVOResult.MapToTestContactDetailsVODto(_mapper);
        }
    }
}