using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests.GetOptionalToManyDestById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOptionalToManyDestByIdQueryHandler : IRequestHandler<GetOptionalToManyDestByIdQuery, OptionalToManyDestDto>
    {
        private readonly IOptionalToManyDestRepository _optionalToManyDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetOptionalToManyDestByIdQueryHandler(IOptionalToManyDestRepository optionalToManyDestRepository,
            IMapper mapper)
        {
            _optionalToManyDestRepository = optionalToManyDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OptionalToManyDestDto> Handle(
            GetOptionalToManyDestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var optionalToManyDest = await _optionalToManyDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (optionalToManyDest is null)
            {
                throw new NotFoundException($"Could not find OptionalToManyDest '{request.Id}'");
            }

            return optionalToManyDest.MapToOptionalToManyDestDto(_mapper);
        }
    }
}