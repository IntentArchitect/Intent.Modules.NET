using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.GetResponseCodes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetResponseCodesQueryHandler : IRequestHandler<GetResponseCodesQuery, List<ResponseCodeDto>>
    {
        private readonly IResponseCodeRepository _responseCodeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetResponseCodesQueryHandler(IResponseCodeRepository responseCodeRepository, IMapper mapper)
        {
            _responseCodeRepository = responseCodeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ResponseCodeDto>> Handle(GetResponseCodesQuery request, CancellationToken cancellationToken)
        {
            var responseCodes = await _responseCodeRepository.FindAllAsync(cancellationToken);
            return responseCodes.MapToResponseCodeDtoList(_mapper);
        }
    }
}