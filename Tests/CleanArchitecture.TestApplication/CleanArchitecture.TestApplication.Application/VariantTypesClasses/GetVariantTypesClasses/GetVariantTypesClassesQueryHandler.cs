using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.GetVariantTypesClasses
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetVariantTypesClassesQueryHandler : IRequestHandler<GetVariantTypesClassesQuery, List<VariantTypesClassDto>>
    {
        private readonly IVariantTypesClassRepository _variantTypesClassRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetVariantTypesClassesQueryHandler(IVariantTypesClassRepository variantTypesClassRepository, IMapper mapper)
        {
            _variantTypesClassRepository = variantTypesClassRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<VariantTypesClassDto>> Handle(GetVariantTypesClassesQuery request, CancellationToken cancellationToken)
        {
            var variantTypesClasses = await _variantTypesClassRepository.FindAllAsync(cancellationToken);
            return variantTypesClasses.MapToVariantTypesClassDtoList(_mapper);
        }
    }
}