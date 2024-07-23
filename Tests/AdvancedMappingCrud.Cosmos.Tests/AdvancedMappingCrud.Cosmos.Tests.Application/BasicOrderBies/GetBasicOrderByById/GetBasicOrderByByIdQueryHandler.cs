using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.GetBasicOrderByById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasicOrderByByIdQueryHandler : IRequestHandler<GetBasicOrderByByIdQuery, BasicOrderByDto>
    {
        private readonly IBasicOrderByRepository _basicOrderByRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBasicOrderByByIdQueryHandler(IBasicOrderByRepository basicOrderByRepository, IMapper mapper)
        {
            _basicOrderByRepository = basicOrderByRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BasicOrderByDto> Handle(GetBasicOrderByByIdQuery request, CancellationToken cancellationToken)
        {
            var basicOrderBy = await _basicOrderByRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basicOrderBy is null)
            {
                throw new NotFoundException($"Could not find BasicOrderBy '{request.Id}'");
            }
            return basicOrderBy.MapToBasicOrderByDto(_mapper);
        }
    }
}