using System.Collections.Generic;
using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Products.GetProducts
{
    public class GetProductsQuery : IRequest<List<ProductDto>>, IQuery
    {
        public GetProductsQuery()
        {
        }
    }
}