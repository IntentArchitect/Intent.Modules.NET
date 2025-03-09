using System;
using GrpcServer.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GrpcServer.Application.Products.CreateComplexProduct
{
    public class CreateComplexProductCommand : IRequest<Guid>, ICommand
    {
        public CreateComplexProductCommand(string name, TypeTestDto typeTestField)
        {
            Name = name;
            TypeTestField = typeTestField;
        }

        public string Name { get; set; }
        public TypeTestDto TypeTestField { get; set; }
    }
}