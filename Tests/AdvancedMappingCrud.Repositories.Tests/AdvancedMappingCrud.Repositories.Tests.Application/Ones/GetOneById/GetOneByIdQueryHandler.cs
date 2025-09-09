using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.NullableNested;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.GetOneById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneByIdQueryHandler : IRequestHandler<GetOneByIdQuery, OneDto>
    {
        private readonly IOneRepository _oneRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOneByIdQueryHandler(IOneRepository oneRepository, IMapper mapper)
        {
            _oneRepository = oneRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OneDto> Handle(GetOneByIdQuery request, CancellationToken cancellationToken)
        {
            var one = await _oneRepository.FindByIdAsync(request.Id, cancellationToken);
            if (one is null)
            {
                throw new NotFoundException($"Could not find One '{request.Id}'");
            }
            return one.MapToOneDto(_mapper);
        }
    }
}