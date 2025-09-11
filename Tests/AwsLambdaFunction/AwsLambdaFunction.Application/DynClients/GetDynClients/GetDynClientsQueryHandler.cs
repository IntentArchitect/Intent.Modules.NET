using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients.GetDynClients
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDynClientsQueryHandler : IRequestHandler<GetDynClientsQuery, List<DynClientDto>>
    {
        private readonly IDynClientRepository _dynClientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDynClientsQueryHandler(IDynClientRepository dynClientRepository, IMapper mapper)
        {
            _dynClientRepository = dynClientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DynClientDto>> Handle(GetDynClientsQuery request, CancellationToken cancellationToken)
        {
            var dynClients = await _dynClientRepository.FindAllAsync(cancellationToken);
            return dynClients.MapToDynClientDtoList(_mapper);
        }
    }
}