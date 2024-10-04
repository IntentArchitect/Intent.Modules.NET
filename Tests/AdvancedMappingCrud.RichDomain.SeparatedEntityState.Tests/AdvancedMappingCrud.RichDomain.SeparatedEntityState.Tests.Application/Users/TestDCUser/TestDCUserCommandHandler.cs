using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users.TestDCUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestDCUserCommandHandler : IRequestHandler<TestDCUserCommand, TestAddressDCDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public TestDCUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestAddressDCDto?> Handle(TestDCUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{request.Id}'");
            }

            var testDCResult = user.TestDC(request.Index);
            return testDCResult?.MapToTestAddressDCDto(_mapper);
        }
    }
}