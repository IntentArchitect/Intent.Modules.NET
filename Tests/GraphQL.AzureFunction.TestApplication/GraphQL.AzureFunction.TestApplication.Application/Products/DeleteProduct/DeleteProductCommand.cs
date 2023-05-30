using System;
using System.Collections.Generic;
using GraphQL.AzureFunction.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Products.DeleteProduct
{
    public class DeleteProductCommand : IRequest<ProductDto>, ICommand
    {
        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }

    }
}