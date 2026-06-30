using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMappingTest.Domain.Common.Exceptions;
using ObjectMappingTest.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ObjectMappingTest.Application.Products.GetDigitalProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDigitalProductByIdHandler : IRequestHandler<GetDigitalProductById, DigitalProductDto>
    {
        private readonly IDigitalProductRepository _digitalProductRepository;

        [IntentManaged(Mode.Merge)]
        public GetDigitalProductByIdHandler(IDigitalProductRepository digitalProductRepository)
        {
            _digitalProductRepository = digitalProductRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<DigitalProductDto> Handle(GetDigitalProductById request, CancellationToken cancellationToken)
        {
            var product = await _digitalProductRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null) throw new NotFoundException($"Could not find DigitalProduct '{request.Id}'");
            return product.MapToDigitalProductDto();
        }
    }
}