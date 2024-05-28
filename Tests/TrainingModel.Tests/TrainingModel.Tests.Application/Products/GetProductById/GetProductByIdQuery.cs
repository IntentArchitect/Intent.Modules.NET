using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products.GetProductById
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