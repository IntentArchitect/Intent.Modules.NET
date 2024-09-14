using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Application.Common.Mappings;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders
{
    public class CustomerOrderDto : IMapFrom<GetCustomerOrdersResponse>
    {
        public CustomerOrderDto()
        {
            RefNo = null!;
        }

        public DateTime OrderDate { get; set; }
        public string RefNo { get; set; }

        public static CustomerOrderDto Create(DateTime orderDate, string refNo)
        {
            return new CustomerOrderDto
            {
                OrderDate = orderDate,
                RefNo = refNo
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GetCustomerOrdersResponse, CustomerOrderDto>();
        }
    }
}