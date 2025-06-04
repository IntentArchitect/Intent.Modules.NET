using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.Sample.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MudBlazor.Sample.Application.Products.GetProductsLookup
{
    public class GetProductsLookupQuery : IRequest<List<ProductDto>>, IQuery
    {
        public GetProductsLookupQuery()
        {
        }
    }
}