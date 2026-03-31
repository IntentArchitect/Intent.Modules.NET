using System.Collections.Generic;
using DtoSettings.Class.Internal.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Defaults.CollectionInput
{
    public class CollectionInputCommand : IRequest, ICommand
    {
        public CollectionInputCommand(List<string>? collection = null)
        {
            Collection = collection ?? [];
        }

        public List<string> Collection { get; set; }
    }
}