using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.GetNoReturnById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNoReturnByIdQueryHandler : IRequestHandler<GetNoReturnByIdQuery, NoReturnDto>
    {
        private readonly INoReturnRepository _noReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetNoReturnByIdQueryHandler(INoReturnRepository noReturnRepository, IMapper mapper)
        {
            _noReturnRepository = noReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<NoReturnDto> Handle(GetNoReturnByIdQuery request, CancellationToken cancellationToken)
        {
            var noReturn = await _noReturnRepository.FindByIdAsync(request.Id, cancellationToken);
            if (noReturn is null)
            {
                throw new NotFoundException($"Could not find NoReturn '{request.Id}'");
            }
            return noReturn.MapToNoReturnDto(_mapper);
        }
    }
}