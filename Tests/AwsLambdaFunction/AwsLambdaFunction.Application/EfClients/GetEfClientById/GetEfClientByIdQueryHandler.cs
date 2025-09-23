using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AwsLambdaFunction.Domain.Common.Exceptions;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients.GetEfClientById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfClientByIdQueryHandler : IRequestHandler<GetEfClientByIdQuery, EfClientDto>
    {
        private readonly IEfClientRepository _efClientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEfClientByIdQueryHandler(IEfClientRepository efClientRepository, IMapper mapper)
        {
            _efClientRepository = efClientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EfClientDto> Handle(GetEfClientByIdQuery request, CancellationToken cancellationToken)
        {
            var efClient = await _efClientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (efClient is null)
            {
                throw new NotFoundException($"Could not find EfClient '{request.Id}'");
            }
            return efClient.MapToEfClientDto(_mapper);
        }
    }
}