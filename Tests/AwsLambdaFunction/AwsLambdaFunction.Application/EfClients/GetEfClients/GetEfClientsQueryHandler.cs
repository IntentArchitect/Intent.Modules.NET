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

namespace AwsLambdaFunction.Application.EfClients.GetEfClients
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfClientsQueryHandler : IRequestHandler<GetEfClientsQuery, List<EfClientDto>>
    {
        private readonly IEfClientRepository _efClientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEfClientsQueryHandler(IEfClientRepository efClientRepository, IMapper mapper)
        {
            _efClientRepository = efClientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EfClientDto>> Handle(GetEfClientsQuery request, CancellationToken cancellationToken)
        {
            var efClients = await _efClientRepository.FindAllAsync(cancellationToken);
            return efClients.MapToEfClientDtoList(_mapper);
        }
    }
}