using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySourceOneToManyDests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToManySourceOneToManyDestsQueryHandler : IRequestHandler<GetOneToManySourceOneToManyDestsQuery, List<OneToManySourceOneToManyDestDto>>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToManySourceOneToManyDestsQueryHandler(IOneToManySourceRepository oneToManySourceRepository,
            IMapper mapper)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OneToManySourceOneToManyDestDto>> Handle(
            GetOneToManySourceOneToManyDestsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _oneToManySourceRepository.FindByIdAsync(request.OwnerId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(OneToManySource)} of Id '{request.OwnerId}' could not be found");
            }
            return aggregateRoot.Owneds.MapToOneToManySourceOneToManyDestDtoList(_mapper);
        }
    }
}