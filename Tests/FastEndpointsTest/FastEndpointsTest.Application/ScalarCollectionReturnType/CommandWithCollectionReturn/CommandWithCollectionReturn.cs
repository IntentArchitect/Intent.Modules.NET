using System.Collections.Generic;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.ScalarCollectionReturnType.CommandWithCollectionReturn
{
    public class CommandWithCollectionReturn : IRequest<List<string>>, ICommand
    {
        public CommandWithCollectionReturn()
        {
        }
    }
}