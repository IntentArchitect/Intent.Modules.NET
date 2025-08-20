using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.NullableNested;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.GetOnes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOnesQueryHandler : IRequestHandler<GetOnesQuery, List<OneDto>>
    {
        private readonly IOneRepository _oneRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOnesQueryHandler(IOneRepository oneRepository, IMapper mapper)
        {
            _oneRepository = oneRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OneDto>> Handle(GetOnesQuery request, CancellationToken cancellationToken)
        {
            var ones = await _oneRepository.FindAllAsync(cancellationToken);
            return ones.MapToOneDtoList(_mapper);
        }
    }
}