using System;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Products.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductDto>, IQuery
    {
        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}