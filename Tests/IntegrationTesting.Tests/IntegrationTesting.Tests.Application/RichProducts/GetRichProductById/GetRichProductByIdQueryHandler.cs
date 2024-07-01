using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.GetRichProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetRichProductByIdQueryHandler : IRequestHandler<GetRichProductByIdQuery, RichProductDto>
    {
        private readonly IRichProductRepository _richProductRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetRichProductByIdQueryHandler(IRichProductRepository richProductRepository, IMapper mapper)
        {
            _richProductRepository = richProductRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<RichProductDto> Handle(GetRichProductByIdQuery request, CancellationToken cancellationToken)
        {
            var richProduct = await _richProductRepository.FindByIdAsync(request.Id, cancellationToken);
            if (richProduct is null)
            {
                throw new NotFoundException($"Could not find RichProduct '{request.Id}'");
            }
            return richProduct.MapToRichProductDto(_mapper);
        }
    }
}