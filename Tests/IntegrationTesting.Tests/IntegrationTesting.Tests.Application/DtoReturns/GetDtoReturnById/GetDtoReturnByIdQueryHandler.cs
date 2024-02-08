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

namespace IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturnById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDtoReturnByIdQueryHandler : IRequestHandler<GetDtoReturnByIdQuery, DtoReturnDto>
    {
        private readonly IDtoReturnRepository _dtoReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDtoReturnByIdQueryHandler(IDtoReturnRepository dtoReturnRepository, IMapper mapper)
        {
            _dtoReturnRepository = dtoReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DtoReturnDto> Handle(GetDtoReturnByIdQuery request, CancellationToken cancellationToken)
        {
            var dtoReturn = await _dtoReturnRepository.FindByIdAsync(request.Id, cancellationToken);
            if (dtoReturn is null)
            {
                throw new NotFoundException($"Could not find DtoReturn '{request.Id}'");
            }
            return dtoReturn.MapToDtoReturnDto(_mapper);
        }
    }
}