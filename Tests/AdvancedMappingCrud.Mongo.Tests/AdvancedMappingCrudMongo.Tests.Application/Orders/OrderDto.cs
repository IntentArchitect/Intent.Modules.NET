using System;
using System.Collections.Generic;
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
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            Id = null!;
            CustomerId = null!;
            RefNo = null!;
            ExternalRef = null!;
            Customer = null!;
            OrderItems = null!;
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ExternalRef { get; set; }
        public OrderCustomerDto Customer { get; set; }
        public List<OrderOrderItemDto> OrderItems { get; set; }

        public static OrderDto Create(
            string id,
            string customerId,
            string refNo,
            DateTime orderDate,
            string externalRef,
            OrderCustomerDto customer,
            List<OrderOrderItemDto> orderItems)
        {
            return new OrderDto
            {
                Id = id,
                CustomerId = customerId,
                RefNo = refNo,
                OrderDate = orderDate,
                ExternalRef = externalRef,
                Customer = customer,
                OrderItems = orderItems
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<Order, OrderDto>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IMapper _mapper;

            public MappingAction(ICustomerRepository customerRepository, IMapper mapper)
            {
                _customerRepository = customerRepository;
                _mapper = mapper;
            }

            public void Process(Order source, OrderDto destination, ResolutionContext context)
            {
                var customer = _customerRepository.FindByIdAsync(source.CustomerId).Result;

                if (customer == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({source.CustomerId}). (Order)->(Customer)");
                }
                destination.Customer = customer.MapToOrderCustomerDto(_mapper);
            }
        }
    }
}