using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.CreatePrivilege
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePrivilegeCommandHandler : IRequestHandler<CreatePrivilegeCommand, PrivilegeDto>
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CreatePrivilegeCommandHandler(IPrivilegeRepository privilegeRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PrivilegeDto> Handle(CreatePrivilegeCommand request, CancellationToken cancellationToken)
        {
            var newPrivilege = new Privilege
            {
                Name = request.Name,
                Description = request.Description,
            };

            _privilegeRepository.Add(newPrivilege);
            await _privilegeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newPrivilege.MapToPrivilegeDto(_mapper);
        }
    }
}