using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.GetVariantTypesClassById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetVariantTypesClassByIdQueryHandler : IRequestHandler<GetVariantTypesClassByIdQuery, VariantTypesClassDto>
    {
        private readonly IVariantTypesClassRepository _variantTypesClassRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetVariantTypesClassByIdQueryHandler(IVariantTypesClassRepository variantTypesClassRepository, IMapper mapper)
        {
            _variantTypesClassRepository = variantTypesClassRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<VariantTypesClassDto> Handle(GetVariantTypesClassByIdQuery request, CancellationToken cancellationToken)
        {
            var variantTypesClass = await _variantTypesClassRepository.FindByIdAsync(request.Id, cancellationToken);
            return variantTypesClass.MapToVariantTypesClassDto(_mapper);
        }
    }
}