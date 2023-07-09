using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Common.Exceptions;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivilegeById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPrivilegeByIdQueryHandler : IRequestHandler<GetPrivilegeByIdQuery, PrivilegeDto>
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetPrivilegeByIdQueryHandler(IPrivilegeRepository privilegeRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PrivilegeDto> Handle(GetPrivilegeByIdQuery request, CancellationToken cancellationToken)
        {
            var privilege = await _privilegeRepository.FindByIdAsync(request.Id, cancellationToken);

            if (privilege is null)
            {
                throw new NotFoundException($"Could not find Privilege '{request.Id}'");
            }
            return privilege.MapToPrivilegeDto(_mapper);
        }
    }
}