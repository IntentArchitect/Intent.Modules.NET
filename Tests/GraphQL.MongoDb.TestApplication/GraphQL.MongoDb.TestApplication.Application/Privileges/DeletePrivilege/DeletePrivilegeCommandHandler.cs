using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.DeletePrivilege
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeletePrivilegeCommandHandler : IRequestHandler<DeletePrivilegeCommand, PrivilegeDto>
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public DeletePrivilegeCommandHandler(IPrivilegeRepository privilegeRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PrivilegeDto> Handle(DeletePrivilegeCommand request, CancellationToken cancellationToken)
        {
            var existingPrivilege = await _privilegeRepository.FindByIdAsync(request.Id, cancellationToken);
            _privilegeRepository.Remove(existingPrivilege);
            return existingPrivilege.MapToPrivilegeDto(_mapper);
        }
    }
}