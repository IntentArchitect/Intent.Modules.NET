using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.GetResponseCodeById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetResponseCodeByIdQueryHandler : IRequestHandler<GetResponseCodeByIdQuery, ResponseCodeDto>
    {
        private readonly IResponseCodeRepository _responseCodeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetResponseCodeByIdQueryHandler(IResponseCodeRepository responseCodeRepository, IMapper mapper)
        {
            _responseCodeRepository = responseCodeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ResponseCodeDto> Handle(GetResponseCodeByIdQuery request, CancellationToken cancellationToken)
        {
            var responseCode = await _responseCodeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (responseCode is null)
            {
                throw new NotFoundException($"Could not find ResponseCode '{request.Id}'");
            }

            return responseCode.MapToResponseCodeDto(_mapper);
        }
    }
}