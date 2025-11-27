using System;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.MixInvocations.CreateItem
{
    public class CreateItemCommand : IRequest<Guid>, ICommand
    {
        public CreateItemCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}