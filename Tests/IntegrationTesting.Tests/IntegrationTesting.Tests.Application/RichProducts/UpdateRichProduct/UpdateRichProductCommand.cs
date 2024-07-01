using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.UpdateRichProduct
{
    public class UpdateRichProductCommand : IRequest, ICommand
    {
        public UpdateRichProductCommand(Guid brandId, string name, Guid id)
        {
            BrandId = brandId;
            Name = name;
            Id = id;
        }

        public Guid BrandId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}