using GrpcServer.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GrpcServer.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, TypeTestDto typeTestField)
        {
            Name = name;
            TypeTestField = typeTestField;
        }

        public string Name { get; set; }
        public TypeTestDto TypeTestField { get; set; }
    }
}