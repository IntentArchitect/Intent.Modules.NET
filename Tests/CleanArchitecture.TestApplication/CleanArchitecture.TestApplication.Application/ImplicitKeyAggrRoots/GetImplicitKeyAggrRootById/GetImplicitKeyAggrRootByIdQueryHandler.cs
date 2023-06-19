using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootByIdQueryHandler : IRequestHandler<GetImplicitKeyAggrRootByIdQuery, ImplicitKeyAggrRootDto>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetImplicitKeyAggrRootByIdQueryHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository, IMapper mapper)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ImplicitKeyAggrRootDto> Handle(
            GetImplicitKeyAggrRootByIdQuery request,
            CancellationToken cancellationToken)
        {
            var implicitKeyAggrRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.Id, cancellationToken);

            if (implicitKeyAggrRoot is null)
            {
                throw new NotFoundException($"Could not find ImplicitKeyAggrRoot {request.Id}");
            }
            return implicitKeyAggrRoot.MapToImplicitKeyAggrRootDto(_mapper);
        }
    }
}