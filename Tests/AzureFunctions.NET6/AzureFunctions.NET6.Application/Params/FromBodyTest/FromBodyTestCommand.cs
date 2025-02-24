using System.Collections.Generic;
using AzureFunctions.NET6.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Params.FromBodyTest
{
    public class FromBodyTestCommand : IRequest, ICommand
    {
        public FromBodyTestCommand(List<int> ids)
        {
            Ids = ids;
        }

        public List<int> Ids { get; set; }
    }
}