using System.Collections.Generic;
using GrpcServer.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace GrpcServer.Application.Products.GetProducts
{
    public class GetProductsQuery : IRequest<List<ProductDto>>, IQuery
    {
        public GetProductsQuery()
        {
        }
    }
}