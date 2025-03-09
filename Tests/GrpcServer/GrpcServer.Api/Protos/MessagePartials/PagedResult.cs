using System.Collections.Generic;
using System.Linq;
using GrpcServer.Api.Protos.Messages.Products;
using GrpcServer.Application;
using GrpcServer.Application.Common.Pagination;
using GrpcServer.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.PagedResultPartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages
{
    public partial class ListOfPagedResultOfComplexTypeDto
    {
        public List<Application.Common.Pagination.PagedResult<Application.ComplexTypeDto>> ToContract()
        {
            return Items.Select(x => x.ToContract()).ToList();
        }

        public static ListOfPagedResultOfComplexTypeDto Create(List<Application.Common.Pagination.PagedResult<Application.ComplexTypeDto>> contract)
        {
            var message = new ListOfPagedResultOfComplexTypeDto();

            if (contract != null)
            {
                message.Items.AddRange(contract.Select(PagedResultOfComplexTypeDto.Create));
            }
            return message;
        }
    }

    public partial class PagedResultOfComplexTypeDto
    {
        public Application.Common.Pagination.PagedResult<Application.ComplexTypeDto> ToContract()
        {
            return new Application.Common.Pagination.PagedResult<Application.ComplexTypeDto>
            {
                TotalCount = TotalCount,
                PageCount = PageCount,
                PageSize = PageSize,
                PageNumber = PageNumber,
                Data = Data.Select(x => x.ToContract()).ToArray()
            };
        }

        public static PagedResultOfComplexTypeDto Create(Application.Common.Pagination.PagedResult<Application.ComplexTypeDto> contract)
        {
            var message = new PagedResultOfComplexTypeDto();
            message.TotalCount = contract.TotalCount;
            message.PageCount = contract.PageCount;
            message.PageSize = contract.PageSize;
            message.PageNumber = contract.PageNumber;
            message.Data.AddRange(contract.Data.Select(ComplexTypeDto.Create));
            return message;
        }
    }

    public partial class PagedResultOfProductDto
    {
        public Application.Common.Pagination.PagedResult<Application.Products.ProductDto> ToContract()
        {
            return new Application.Common.Pagination.PagedResult<Application.Products.ProductDto>
            {
                TotalCount = TotalCount,
                PageCount = PageCount,
                PageSize = PageSize,
                PageNumber = PageNumber,
                Data = Data.Select(x => x.ToContract()).ToArray()
            };
        }

        public static PagedResultOfProductDto Create(Application.Common.Pagination.PagedResult<Application.Products.ProductDto> contract)
        {
            var message = new PagedResultOfProductDto();
            message.TotalCount = contract.TotalCount;
            message.PageCount = contract.PageCount;
            message.PageSize = contract.PageSize;
            message.PageNumber = contract.PageNumber;
            message.Data.AddRange(contract.Data.Select(Products.ProductDto.Create));
            return message;
        }
    }
}