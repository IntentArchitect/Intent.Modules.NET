using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Brands.GetBrands
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, List<BrandDto>>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBrandsQueryHandler(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandRepository.FindAllAsync(cancellationToken);
            return brands.MapToBrandDtoList(_mapper);
        }
    }
}