using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Application.Interfaces;
using FastEndpointsTest.Application.SimpleProducts;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class SimpleProductsService : ISimpleProductsService
    {
        private readonly ISimpleProductRepository _simpleProductRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public SimpleProductsService(ISimpleProductRepository simpleProductRepository, IMapper mapper)
        {
            _simpleProductRepository = simpleProductRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateSimpleProduct(
            SimpleProductCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var simpleProduct = new SimpleProduct
            {
                Name = dto.Name,
                Value = dto.Value
            };

            _simpleProductRepository.Add(simpleProduct);
            await _simpleProductRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return simpleProduct.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateSimpleProduct(
            Guid id,
            SimpleProductUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var simpleProduct = await _simpleProductRepository.FindByIdAsync(id, cancellationToken);
            if (simpleProduct is null)
            {
                throw new NotFoundException($"Could not find SimpleProduct '{id}'");
            }

            simpleProduct.Name = dto.Name;
            simpleProduct.Value = dto.Value;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SimpleProductDto> FindSimpleProductById(Guid id, CancellationToken cancellationToken = default)
        {
            var simpleProduct = await _simpleProductRepository.FindByIdAsync(id, cancellationToken);
            if (simpleProduct is null)
            {
                throw new NotFoundException($"Could not find SimpleProduct '{id}'");
            }
            return simpleProduct.MapToSimpleProductDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SimpleProductDto>> FindSimpleProducts(CancellationToken cancellationToken = default)
        {
            var simpleProducts = await _simpleProductRepository.FindAllAsync(cancellationToken);
            return simpleProducts.MapToSimpleProductDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteSimpleProduct(Guid id, CancellationToken cancellationToken = default)
        {
            var simpleProduct = await _simpleProductRepository.FindByIdAsync(id, cancellationToken);
            if (simpleProduct is null)
            {
                throw new NotFoundException($"Could not find SimpleProduct '{id}'");
            }

            _simpleProductRepository.Remove(simpleProduct);
        }
    }
}