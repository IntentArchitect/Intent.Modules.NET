using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Application.Products.GetProducts
{
    public class GetProductsQuery : IQuery
    {
        public GetProductsQuery()
        {
        }
    }
}