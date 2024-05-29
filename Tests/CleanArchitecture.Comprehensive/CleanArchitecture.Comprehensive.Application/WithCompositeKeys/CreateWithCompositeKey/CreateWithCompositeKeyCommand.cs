using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.CreateWithCompositeKey
{
    public class CreateWithCompositeKeyCommand : IRequest<Guid>, ICommand
    {
        public CreateWithCompositeKeyCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}