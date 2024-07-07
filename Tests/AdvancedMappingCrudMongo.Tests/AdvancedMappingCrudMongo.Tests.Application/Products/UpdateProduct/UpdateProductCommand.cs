using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Products.UpdateProduct
{
    public class UpdateProductCommand : IRequest, ICommand
    {
        public UpdateProductCommand(string name, string description, string id)
        {
            Name = name;
            Description = description;
            Id = id;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
    }
}