using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivileges
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPrivilegesQueryHandler : IRequestHandler<GetPrivilegesQuery, List<PrivilegeDto>>
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetPrivilegesQueryHandler(IPrivilegeRepository privilegeRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<PrivilegeDto>> Handle(GetPrivilegesQuery request, CancellationToken cancellationToken)
        {
            var privileges = await _privilegeRepository.FindAllAsync(cancellationToken);
            return privileges.MapToPrivilegeDtoList(_mapper);
        }
    }
}