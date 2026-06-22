using System;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AzureFunctions.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Products.GetProducts
{
    public class GetProductsQuery : IQuery
    {
        public GetProductsQuery()
        {
        }
    }
}