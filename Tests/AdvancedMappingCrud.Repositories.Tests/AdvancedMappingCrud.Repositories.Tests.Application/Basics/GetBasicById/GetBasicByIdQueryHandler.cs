using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasicById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasicByIdQueryHandler : IRequestHandler<GetBasicByIdQuery, BasicDto>
    {
        private readonly IBasicRepository _basicRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBasicByIdQueryHandler(IBasicRepository basicRepository, IMapper mapper)
        {
            _basicRepository = basicRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BasicDto> Handle(GetBasicByIdQuery request, CancellationToken cancellationToken)
        {
            var basic = await _basicRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basic is null)
            {
                throw new NotFoundException($"Could not find Basic '{request.Id}'");
            }
            return basic.MapToBasicDto(_mapper);
        }
    }
}