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

namespace IntegrationTesting.Tests.Application.UniqueConVals.GetUniqueConValById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUniqueConValByIdQueryHandler : IRequestHandler<GetUniqueConValByIdQuery, UniqueConValDto>
    {
        private readonly IUniqueConValRepository _uniqueConValRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetUniqueConValByIdQueryHandler(IUniqueConValRepository uniqueConValRepository, IMapper mapper)
        {
            _uniqueConValRepository = uniqueConValRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UniqueConValDto> Handle(GetUniqueConValByIdQuery request, CancellationToken cancellationToken)
        {
            var uniqueConVal = await _uniqueConValRepository.FindByIdAsync(request.Id, cancellationToken);
            if (uniqueConVal is null)
            {
                throw new NotFoundException($"Could not find UniqueConVal '{request.Id}'");
            }
            return uniqueConVal.MapToUniqueConValDto(_mapper);
        }
    }
}