using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.CreateOptionalToOneDest
{
    public class CreateOptionalToOneDestCommand : IRequest<Guid>, ICommand
    {
        public CreateOptionalToOneDestCommand(string attribute)
        {
            Attribute = attribute;
        }

        public string Attribute { get; set; }
    }
}