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

namespace AwsLambdaFunction.Application.DynClients.GetDynClientById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDynClientByIdQueryHandler : IRequestHandler<GetDynClientByIdQuery, DynClientDto>
    {
        private readonly IDynClientRepository _dynClientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDynClientByIdQueryHandler(IDynClientRepository dynClientRepository, IMapper mapper)
        {
            _dynClientRepository = dynClientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DynClientDto> Handle(GetDynClientByIdQuery request, CancellationToken cancellationToken)
        {
            var dynClient = await _dynClientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (dynClient is null)
            {
                throw new NotFoundException($"Could not find DynClient '{request.Id}'");
            }
            return dynClient.MapToDynClientDto(_mapper);
        }
    }
}