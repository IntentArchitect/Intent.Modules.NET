using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Optionals.GetOptionalById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalByIdQueryHandler : IRequestHandler<GetOptionalByIdQuery, OptionalDto?>
    {
        private readonly IOptionalRepository _optionalRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOptionalByIdQueryHandler(IOptionalRepository optionalRepository, IMapper mapper)
        {
            _optionalRepository = optionalRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OptionalDto?> Handle(GetOptionalByIdQuery request, CancellationToken cancellationToken)
        {
            var optional = await _optionalRepository.FindByIdAsync(request.Id, cancellationToken);
            return optional?.MapToOptionalDto(_mapper);
        }
    }
}