using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class ListOfProductDto
    {
        public List<Application.Products.ProductDto> ToContract()
        {
            return Items.Select(x => x.ToContract()).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfProductDto? Create(List<Application.Products.ProductDto>? contract)
        {
            if (contract == null)
            {
                return null;
            }
            var message = new ListOfProductDto();
            message.Items.AddRange(contract.Select(ProductDto.Create));
            return message;
        }
    }

    public partial class ProductDto
    {
        public Application.Products.ProductDto ToContract()
        {
            return new Application.Products.ProductDto
            {
                Id = Guid.Parse(Id),
                Name = Name
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ProductDto? Create(Application.Products.ProductDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ProductDto
            {
                Id = contract.Id.ToString(),
                Name = contract.Name
            };

            return message;
        }
    }
}