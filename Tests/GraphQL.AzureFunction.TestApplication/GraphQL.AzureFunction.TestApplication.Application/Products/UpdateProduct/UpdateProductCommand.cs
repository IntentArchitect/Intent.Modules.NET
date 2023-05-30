using System;
using System.Collections.Generic;
using GraphQL.AzureFunction.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Products.UpdateProduct
{
    public class UpdateProductCommand : IRequest<ProductDto>, ICommand
    {
        public UpdateProductCommand(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

    }
}