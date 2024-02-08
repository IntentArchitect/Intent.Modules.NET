using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturns
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDtoReturnsQueryHandler : IRequestHandler<GetDtoReturnsQuery, List<DtoReturnDto>>
    {
        private readonly IDtoReturnRepository _dtoReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDtoReturnsQueryHandler(IDtoReturnRepository dtoReturnRepository, IMapper mapper)
        {
            _dtoReturnRepository = dtoReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DtoReturnDto>> Handle(GetDtoReturnsQuery request, CancellationToken cancellationToken)
        {
            var dtoReturns = await _dtoReturnRepository.FindAllAsync(cancellationToken);
            return dtoReturns.MapToDtoReturnDtoList(_mapper);
        }
    }
}