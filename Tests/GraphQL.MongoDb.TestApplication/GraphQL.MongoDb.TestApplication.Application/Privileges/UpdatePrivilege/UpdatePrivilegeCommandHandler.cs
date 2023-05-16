using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.UpdatePrivilege
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePrivilegeCommandHandler : IRequestHandler<UpdatePrivilegeCommand, PrivilegeDto>
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public UpdatePrivilegeCommandHandler(IPrivilegeRepository privilegeRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PrivilegeDto> Handle(UpdatePrivilegeCommand request, CancellationToken cancellationToken)
        {
            var existingPrivilege = await _privilegeRepository.FindByIdAsync(request.Id, cancellationToken);
            existingPrivilege.Name = request.Name;
            existingPrivilege.Description = request.Description;
            return existingPrivilege.MapToPrivilegeDto(_mapper);
        }
    }
}