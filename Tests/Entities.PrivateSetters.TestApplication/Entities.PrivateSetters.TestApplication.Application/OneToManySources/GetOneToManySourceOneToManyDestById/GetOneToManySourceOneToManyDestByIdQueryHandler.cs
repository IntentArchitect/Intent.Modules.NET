using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySourceOneToManyDestById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToManySourceOneToManyDestByIdQueryHandler : IRequestHandler<GetOneToManySourceOneToManyDestByIdQuery, OneToManySourceOneToManyDestDto>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToManySourceOneToManyDestByIdQueryHandler(IOneToManySourceRepository oneToManySourceRepository,
            IMapper mapper)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OneToManySourceOneToManyDestDto> Handle(
            GetOneToManySourceOneToManyDestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _oneToManySourceRepository.FindByIdAsync(request.OwnerId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(OneToManySource)} of Id '{request.OwnerId}' could not be found");
            }

            var element = aggregateRoot.Owneds.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find OneToManyDest '{request.Id}'");
            }

            return element.MapToOneToManySourceOneToManyDestDto(_mapper);
        }
    }
}