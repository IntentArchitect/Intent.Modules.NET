using System;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AwsLambdaFunctions.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Application.Products.GetProducts
{
    public class GetProductsQuery : IQuery
    {
        public GetProductsQuery()
        {
        }
    }
}