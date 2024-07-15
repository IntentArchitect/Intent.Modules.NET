using System.Linq;
using AdvancedMappingCrudMongo.Tests.Application.Common.Mappings;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    public class OrderOrderItemDto : IMapFrom<OrderItem>
    {
        public OrderOrderItemDto()
        {
            ProductId = null!;
            Id = null!;
            Product = null!;
        }

        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public string Id { get; set; }
        public OrderOrderItemProductDto Product { get; set; }

        public static OrderOrderItemDto Create(
            int quantity,
            decimal amount,
            string productId,
            string id,
            OrderOrderItemProductDto product)
        {
            return new OrderOrderItemDto
            {
                Quantity = quantity,
                Amount = amount,
                ProductId = productId,
                Id = id,
                Product = product
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderItem, OrderOrderItemDto>()
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<OrderItem, OrderOrderItemDto>
        {
            private readonly IProductRepository _productRepository;
            private readonly IMapper _mapper;

            public MappingAction(IProductRepository productRepository, IMapper mapper)
            {
                _productRepository = productRepository;
                _mapper = mapper;
            }

            public void Process(OrderItem source, OrderOrderItemDto destination, ResolutionContext context)
            {
                var product = _productRepository.FindByIdAsync(source.ProductId).Result;

                if (product == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({source.ProductId}). (OrderItem)->(Product)");
                }
                destination.Product = product.MapToOrderOrderItemProductDto(_mapper);
            }
        }
    }
}