using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOneToManySourceByIdQueryHandler : IRequestHandler<GetOneToManySourceByIdQuery, OneToManySourceDto>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOneToManySourceByIdQueryHandler(IOneToManySourceRepository oneToManySourceRepository, IMapper mapper)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OneToManySourceDto> Handle(
            GetOneToManySourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var oneToManySource = await _oneToManySourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (oneToManySource is null)
            {
                throw new NotFoundException($"Could not find OneToManySource '{request.Id}'");
            }

            return oneToManySource.MapToOneToManySourceDto(_mapper);
        }
    }
}